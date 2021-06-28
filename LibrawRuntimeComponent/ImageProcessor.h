#include <string>
#pragma once


namespace LibrawRuntimeComponent
{
	using Platform::String;

	public ref class Histogram sealed
	{
	public:
		
		property Platform::Array<int>^ red;
		property Platform::Array<int>^ green;
		property Platform::Array<int>^ blue;

	};

	public value struct LibRawExif
	{
	
		float       iso_speed;
		float       shutter;
		float       aperture;
		float       focal_len;
		time_t      timestamp;
		int			shot_order;
		float		gps_latitude;
		float		gps_longtitude;
		float		gps_altitude;
		bool		gps_parsed;
		String^     desc;
		String^		author;
		int			bits;
		int			colors;
		String^		model;
		String^		manufacturer;
		float		flash;
		String^		Software;
		bool		empty;

		
	};

	public value struct LibRawUnpackResult
	{
		LibRawExif EXIF;
		int Error;
		int Width; //or float64 if you prefer
		int Height;
	};
	
	public ref class ImageProcessor sealed
	{
	public:
		ImageProcessor::ImageProcessor();
		
		Histogram^ GetHistogram();

		LibRawUnpackResult Process(bool halfsize);
		
		LibRawUnpackResult Unpack(Platform::String^ something, bool autobright, bool autoWB, bool noise);
		Platform::Array<uint8>^ GetImageBytes();
		
		void Cancel();
		
		//Platform::Array<uint8>^ GetImageBytes2();
		
	};
}