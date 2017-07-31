#pragma once

using namespace System;
using namespace System::Reflection;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace PaintDotNet;

namespace FileTypeDDS 
{
	public ref class DDSFileType sealed : public FileType
	{
	public:
		DDSFileType();

	protected:
		// Override the onload
		virtual Document^ OnLoad(Stream^ Input) override;
		
	private:
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
