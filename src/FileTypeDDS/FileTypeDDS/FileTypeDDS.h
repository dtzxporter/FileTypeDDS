#pragma once

using namespace System;
using namespace System::Reflection;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace PaintDotNet;
using namespace PaintDotNet::PropertySystem;
using namespace PaintDotNet::IndirectUI;

namespace FileTypeDDS 
{
	public ref class DDSFileType sealed : public PropertyBasedFileType
	{
	public:
		DDSFileType();

		// Override save functions
		virtual ControlInfo^ OnCreateSaveConfigUI(PropertyCollection^ Props) override;
		virtual PropertyCollection^ OnCreateSavePropertyCollection() override;

	protected:
		// Override the onload and onsave
		virtual Document^ OnLoad(Stream^ Input) override;
		virtual void OnSaveT(Document^ Input, Stream^ Output, PropertyBasedSaveConfigToken^ Token, Surface^ Surface, ProgressEventHandler^ Callback) override;
		
		
	private:
		// An internal reference to our supported file type
		PropertyBasedFileType^ InternalFileType;

		// A list of supported extensions
		static array<String^>^ Extensions = gcnew array<String^>(1) { ".dds" };
	};

	public ref class DDSFileTypes : IFileTypeFactory
	{
	public:
		// Returns the file types
		virtual array<FileType^>^ GetFileTypeInstances();

	private:
		// The DDS file type
		static DDSFileType^ DDSType = gcnew DDSFileType();
		// A list of types
		static array<FileType^>^ FileTypeInstances = gcnew array<FileType^>(1) { DDSType };
	};
}
