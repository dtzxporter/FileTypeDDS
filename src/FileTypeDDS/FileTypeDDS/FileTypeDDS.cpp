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

DDSFileType::DDSFileType() : FileType("DirectDraw Surface (DDS)", FileTypeFlags::SupportsLoading, Extensions)
{
	// Perform initialization here, for this, we're going to reflect into PaintDotNet and verify that the EXE was patched
	Assembly^ CurrentAssembly = Assembly::GetEntryAssembly();
	// Reflect to the "PdnFileTypes" class
	try
	{
		//// Fetch the type
		//auto FileType = CurrentAssembly->GetType("PaintDotNet.Data.PdnFileTypes");

		//auto field = FileType->GetField("Dds");

		//auto res = field->FieldHandle.Value.ToInt64();

		//System::Windows::Forms::MessageBox::Show("fff " + field->Name + " 0x" + res.ToString("X"));
	}
	catch (Exception^ ex)
	{
		// Display the error, this shouldn't happen with release builds
		System::Windows::Forms::MessageBox::Show("FileTypeDDS - Failed to verify installation. (Report this) [" + ex->Message + "]");
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
			DXGI_FORMAT ResultFormat = DXGI_FORMAT_R8G8B8A8_UNORM;
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
		else if (ImageMetadata.format != DXGI_FORMAT::DXGI_FORMAT_R8G8B8A8_UNORM)
		{
			// Allocate a temporary buffer
			auto TemporaryImage = std::make_unique<DirectX::ScratchImage>();

			// We must use this format for global support, it's a standard for images
			DXGI_FORMAT ResultFormat = DXGI_FORMAT_R8G8B8A8_UNORM;
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

		// Prepare a new layer for the document, this will house the RGBA data
		BitmapLayer^ DocumentLayer = Layer::CreateBackgroundLayer((int)ImageMetadata.width, (int)ImageMetadata.height);
		// Color buffer
		ColorBgra ColorBuffer;
		// A pointer to the raw pixels in RGBA format
		auto RawPixels = Image->GetPixels();
		// The index of the pixels
		uint32_t PixelIndex = 0;
		// Loop for height and width and copy over the RGBA pixel data
		for (int i = 0; i < (int)ImageMetadata.height; i++)
		{
			for (int j = 0; j < (int)ImageMetadata.width; j++)
			{
				// Set
				ColorBuffer.R = RawPixels[PixelIndex];
				ColorBuffer.G = RawPixels[PixelIndex + 1];
				ColorBuffer.B = RawPixels[PixelIndex + 2];
				ColorBuffer.A = RawPixels[PixelIndex + 3];
				// Advance
				PixelIndex += 4;
				// Set the item
				DocumentLayer->Surface[j, i] = ColorBuffer;
			}
		}

		// Force clean up
		Image.reset();

		// Make a document
		Document^ ResultDocument = gcnew Document((int)ImageMetadata.width, (int)ImageMetadata.height);
		// Add the layer
		ResultDocument->Layers->Add(DocumentLayer);

		// Return a test doc
		return ResultDocument;
	}

	// We failed to load
	return nullptr;
}

array<FileType^>^ DDSFileTypes::GetFileTypeInstances()
{
	// Return it
	return FileTypeInstances;
}