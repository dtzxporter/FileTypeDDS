#include "stdafx.h"

// Disable several warnings about DirectX
#pragma warning(disable : 4561)
#pragma warning(disable : 4945)

// The main part of this library, the DirectXTex library
#include <DirectXTex.h>
#include <memory>

// Required for DirectXTex
#pragma comment(lib, "Ole32.lib")

// The class we are implementing
#include "FileTypeDDS.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime;
using namespace System::Runtime::CompilerServices;
using namespace System::IO;
using namespace FileTypeDDS;
using namespace PaintDotNet;

DDSFileType::DDSFileType() : PropertyBasedFileType("DirectDraw Surface (DDS)", FileTypeFlags::SupportsLoading | FileTypeFlags::SupportsSaving | FileTypeFlags::SavesWithProgress, Extensions)
{
	// Perform initialization here, for this, we're going to reflect into PaintDotNet and borrow the old DDS saving logic
	Assembly^ CurrentAssembly = Assembly::GetEntryAssembly();
	// Reflect to the "PdnFileTypes" class
	try
	{
#if _DEBUG
		System::Windows::Forms::MessageBox::Show("FileTypeDDS -- DEBUG MODE --");
#endif

		// Fetch the type
		auto FileType = CurrentAssembly->GetType("PaintDotNet.Data.Dds.DdsFileType");
		this->InternalFileType = (PropertyBasedFileType^)Activator::CreateInstance(FileType);

#if _DEBUG
		System::Windows::Forms::MessageBox::Show("FileTypeDDS -- Loaded internal file handler --");
#endif
	}
	catch (Exception^ ex)
	{
		// Display the error
		System::Windows::Forms::MessageBox::Show("FileTypeDDS - Failed to initialize plugin. (Report this on Github) [" + ex->Message + "]", "FileTypeDDS");
	}
}

Document^ DDSFileType::OnLoad(Stream^ Input)
{
	// Ensure we're at the beginning
	Input->Seek(0, SeekOrigin::Begin);

	// The full length of the strean
	auto StreamLength = Input->Length;

	// Allocate a .NET buffer for reading
	auto ReadBuffer = gcnew array<Byte>((int)StreamLength);

	// Read the whole file
	auto ResultLength = Input->Read(ReadBuffer, 0, (int)StreamLength);

	// Ensure we at least read something (DirectXTex will attempt to load what it can)
	if (ResultLength > 0)
	{	
		// Allocate a new image for loading
		auto Image = std::make_unique<DirectX::ScratchImage>();
		// Allocate a buffer for metadata
		DirectX::TexMetadata ImageMetadata;

		// Pin down the array
		pin_ptr<Byte> PinnedBuffer = &ReadBuffer[0];
		// Grab the native handle
		unsigned char* NativeHandle = PinnedBuffer;

		// The result of loading the file, defaults to failed
		HRESULT Result = -1;

		// Read the DDS file from memory
		Result = DirectX::LoadFromDDSMemory(NativeHandle, (size_t)ResultLength, DirectX::DDS_FLAGS::DDS_FLAGS_NONE, &ImageMetadata, *Image);

		// Check if we are successful
		if (FAILED(Result))
		{
			// Alert
			throw gcnew FormatException("File does not appear to be a DDS image");
			// Failed
			return nullptr;
		}

		// Stage 1: Check if the image is planar, if so, convert to a single plane
		if (DirectX::IsPlanar(ImageMetadata.format))
		{
			// Fetch first image
			auto FirstImage = Image->GetImage(0, 0, 0);
			// Fetch image count
			auto ImageCount = Image->GetImageCount();
			// Allocate a temporary buffer
			auto TemporaryImage = std::make_unique<DirectX::ScratchImage>();

			// Convert to normal plane
			auto Result = DirectX::ConvertToSinglePlane(FirstImage, ImageCount, ImageMetadata, *TemporaryImage);

			// Ensure success
			if (!FAILED(Result))
			{
				// Get the information
				auto& TextureInfo = TemporaryImage->GetMetadata();
				// Swap out our format
				ImageMetadata.format = TextureInfo.format;
				// Swap the temp image to normal one
				Image.reset(TemporaryImage.release());
			}
			else
			{
				// Alert
				throw gcnew FormatException("Failed to convert planar DDS");
				// Failed to process
				return nullptr;
			}
		}

		// Stage 2: Decompress the texture if necessary, or ensure it's in the proper format
		if (DirectX::IsCompressed(ImageMetadata.format))
		{
			// Fetch first image
			auto FirstImage = Image->GetImage(0, 0, 0);
			// Fetch image count
			auto ImageCount = Image->GetImageCount();
			// Allocate a temporary buffer
			auto TemporaryImage = std::make_unique<DirectX::ScratchImage>();

			// We must use this format for global support, it's a standard for images
			DXGI_FORMAT ResultFormat = DXGI_FORMAT_B8G8R8A8_UNORM;
			// Decompress the image
			auto Result = DirectX::Decompress(FirstImage, ImageCount, ImageMetadata, ResultFormat, *TemporaryImage);

			// Ensure success
			if (!FAILED(Result))
			{
				// Get the information
				auto& TextureInfo = TemporaryImage->GetMetadata();
				// Swap out our format
				ImageMetadata.format = TextureInfo.format;
				// Swap the temp image to normal one
				Image.reset(TemporaryImage.release());
			}
			else
			{
				// Alert
				throw gcnew FormatException("Failed to decompress DDS");
				// Failed to process
				return nullptr;
			}
		}
		else if (ImageMetadata.format != DXGI_FORMAT::DXGI_FORMAT_B8G8R8A8_UNORM)
		{
			// Allocate a temporary buffer
			auto TemporaryImage = std::make_unique<DirectX::ScratchImage>();

			// We must use this format for global support, it's a standard for images
			DXGI_FORMAT ResultFormat = DXGI_FORMAT_B8G8R8A8_UNORM;
			// Convert to our format
			auto Result = DirectX::Convert(Image->GetImages(), Image->GetImageCount(), Image->GetMetadata(), ResultFormat, DirectX::TEX_FILTER_FLAGS::TEX_FILTER_DEFAULT, DirectX::TEX_THRESHOLD_DEFAULT, *TemporaryImage);

			// Ensure success
			if (!FAILED(Result))
			{
				// Get the information
				auto& TextureInfo = TemporaryImage->GetMetadata();
				// Swap out our format
				ImageMetadata.format = TextureInfo.format;
				// Swap the temp image to normal one
				Image.reset(TemporaryImage.release());
			}
			else
			{
				// Alert
				throw gcnew FormatException("Failed to convert DDS");
				// Failed to process
				return nullptr;
			}
		}

		// Prepare a new layer for the document, this will house the BGRA data
		Document^ ResultDocument = gcnew Document((int)ImageMetadata.width, (int)ImageMetadata.height);
		ResultDocument->Layers->Add(Layer::CreateBackgroundLayer((int)ImageMetadata.width, (int)ImageMetadata.height));

		// A pointer to the raw pixels in BGRA format
		auto Buffer = Image->GetPixels();
		auto DestPtr = ((BitmapLayer^)ResultDocument->Layers[0])->Surface->Scan0->VoidStar;

		// Copy the raw pixels
		std::memcpy(DestPtr, Buffer, ImageMetadata.width * ImageMetadata.height * 4);

		// Force clean up
		Image.reset();

		// Return a test doc
		return ResultDocument;
	}

	// We failed to load
	return nullptr;
}

void DDSFileType::OnSaveT(Document^ Input, Stream^ Output, PropertyBasedSaveConfigToken^ Token, Surface^ Surface, ProgressEventHandler^ Callback)
{
	this->InternalFileType->Save(Input, Output, Token, Surface, Callback, false);
}

ControlInfo^ DDSFileType::OnCreateSaveConfigUI(PropertyCollection^ Props)
{
	return this->InternalFileType->OnCreateSaveConfigUI(Props);
}

PropertyCollection^ DDSFileType::OnCreateSavePropertyCollection()
{
	return this->InternalFileType->OnCreateSavePropertyCollection();
}

array<FileType^>^ DDSFileTypes::GetFileTypeInstances()
{
	// Return it
	return FileTypeInstances;
}