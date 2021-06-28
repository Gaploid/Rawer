#include "pch.h"
#include "ImageProcessor.h"
#include <stdio.h>
#include <string.h>
#include <winsock2.h>
#include <string>
#include <math.h>


#include "libraw.h"
using namespace LibrawRuntimeComponent;

bool key_pressed;
LibRaw RawProcessor;
#define P1  RawProcessor.imgdata.idata
#define S   RawProcessor.imgdata.sizes
#define C   RawProcessor.imgdata.color
#define T   RawProcessor.imgdata.thumbnail
#define P2  RawProcessor.imgdata.other
#define OUT RawProcessor.imgdata.params

libraw_processed_image_t *image;
Histogram^ histo;

int my_progress_callback(void *data, enum LibRaw_progress p, int iteration, int expected)
{
	
	//RawProcessor.~LibRaw.strprogress(p);

	/*char *passed_string = (char *data);
	printf("Callback: %s  pass %d of %d, data passed: %s\n", libraw_strprogress(p), iteration, expected, passed_string);*/
	if (key_pressed)
		return 1; // cancel processing immediately
	else
		return 0; // can continue
}

ImageProcessor::ImageProcessor(){

	key_pressed = false;	
	RawProcessor.set_progress_handler(my_progress_callback, NULL);

}

String^ StringFromAscIIChars(char* chars)
{
	size_t newsize = strlen(chars) + 1;
	wchar_t * wcstring = new wchar_t[newsize];
	size_t convertedChars = 0;
	mbstowcs_s(&convertedChars, wcstring, newsize, chars, _TRUNCATE);
	String^ str = ref new Platform::String(wcstring);
	delete[] wcstring;
	return str;
}

Histogram^ ImageProcessor::GetHistogram(){ 	

	return histo;
}


Platform::Array<uint8>^ ImageProcessor::GetImageBytes(){


	//convert from RGB to BGRA (silverlight)
	//int index, index2 = 0;
	//int size = image->height * image->width * 4;

	//Platform::Array<int>^ red = ref new Platform::Array<int>(256);
	//Platform::Array<int>^ green = ref new Platform::Array<int>(256);
	//Platform::Array<int>^ blue = ref new Platform::Array<int>(256);


	//Platform::Array<uint8>^ newarray = ref new Platform::Array<uint8>(size);

	//#pragma omp parallel for private(index, index2)
	//for (int y = 0; y < image->height; y++)
	//{
	//	for (int x = 0; x < image->width; x++)
	//		{

	//			index = (x + y * image->width) * 4;
	//			index2 = (x + y * image->width) * 3;

	//			newarray[index] = image->data[index2 + 2];
	//			newarray[index + 1] = image->data[index2 + 1];
	//			newarray[index + 2] = image->data[index2];
	//			newarray[index + 3] = 0xFF;


	//			//Заполняем для гистограммы
	//			red[image->data[index2]] += 1;
	//			//System.Diagnostics.Debug.WriteLine(array[index2] + "  color count-" + red[array[index2]]);
	//			green[image->data[index2 + 1]] += 1;
	//			blue[image->data[index2 + 2]] += 1;
	//		}
	//}
	//histo = ref new Histogram();
	//histo->blue = blue;
	//histo->green = green;
	//histo->red = red;
	//

	Platform::Array<uint8>^ my = ref new Platform::Array<uint8>(image->data, image->data_size);

	LibRaw::dcraw_clear_mem(image);

	RawProcessor.free_image();

	RawProcessor.recycle();

	return my;
}

void ImageProcessor::Cancel(){

	key_pressed = true;
	RawProcessor.setCancelFlag();

}

LibRawUnpackResult ImageProcessor::Process(bool halfsize){

	//If we need half size image then set param
	if (halfsize){
		OUT.half_size = 1;

	}
	else{
		OUT.half_size = 0;
	}


	LibRawUnpackResult r;

	r.Error = 0;

	int  i, ret, verbose = 0, output_thumbs = 0;



	// Распаковка данных
	ret = RawProcessor.dcraw_process();
	if (ret != LIBRAW_SUCCESS){
		RawProcessor.recycle();
		r.Error = ret;

		return r;
	}

	image = RawProcessor.dcraw_make_mem_image(&ret);

	if (ret != LIBRAW_SUCCESS){
		RawProcessor.recycle();
		r.Error = ret;

		return r;
	}

#define SWAP(a,b) { a ^= b; a ^= (b ^= a); }
	if (image->bits == 16 && htons(0x55aa) != 0x55aa)
		for (unsigned i = 0; i < image->data_size; i += 2)
			SWAP(image->data[i], image->data[i + 1]);
#undef SWAP

	r.Height = image->height;
	r.Width = image->width;
	

	LibRawExif exif;
	if (P2.parsed_gps.gpsparsed){
	
		exif.gps_parsed = P2.parsed_gps.gpsparsed;
		exif.gps_altitude = P2.parsed_gps.altitude;
		exif.gps_latitude = P2.parsed_gps.latitude[0] + P2.parsed_gps.latitude[1] / 60 + P2.parsed_gps.latitude[2] / 60 / 60;
		exif.gps_longtitude = P2.parsed_gps.longtitude[0] + P2.parsed_gps.longtitude[1] / 60 + P2.parsed_gps.longtitude[2] / 60 / 60;
	}

	exif.aperture = P2.aperture;
	exif.author = StringFromAscIIChars(P2.artist);
	exif.desc = StringFromAscIIChars(P2.desc);
	exif.focal_len = P2.focal_len; 
	//exif.gpsdata = (int)P2.gpsdata;
	exif.iso_speed = P2.iso_speed;
	exif.shot_order = P2.shot_order;
	exif.shutter = P2.shutter;
	exif.timestamp = P2.timestamp;
	exif.colors = image->colors;
	exif.bits = image->bits;
	exif.model = StringFromAscIIChars(RawProcessor.imgdata.idata.model);
	exif.flash = RawProcessor.imgdata.color.flash_used;
	exif.manufacturer = StringFromAscIIChars(RawProcessor.imgdata.idata.make);
	exif.Software = StringFromAscIIChars(RawProcessor.imgdata.idata.software);
	exif.empty = false;
	r.EXIF = exif;
	int iii = RawProcessor.imgdata.process_warnings;

	return r;


}

LibRawUnpackResult ImageProcessor::Unpack(Platform::String^ something, bool autobright, bool autoWB, bool noise){


	if (!autobright) { OUT.no_auto_bright = 1; }
	else { OUT.no_auto_bright = 0; }

	if (autoWB) {
		OUT.use_auto_wb = 1;
		OUT.use_camera_wb = 0;
	}
	else{
		OUT.use_auto_wb = 0;
		OUT.use_camera_wb = 1;


	}

	/*0 - linear interpolation
	1 - VNG interpolation
	2 - PPG interpolation
	3 - AHD interpolation
	4 - DCB interpolation
	5 - Modified AHD interpolation by Paul Lee
	6 - AFD interpolation(5 - pass)
	7 - VCD interpolation
	8 - Mixed VCD / Modified AHD interpolation
	9 - LMMSE interpolation
	10 - AMaZE intepolation*/
	OUT.user_qual = 9;  

	OUT.use_camera_matrix = 1;
	OUT.camera_profile = "embed";

	if (noise){
		OUT.threshold = 500;
	}
	else{
		OUT.threshold = 0;

	}



	//noise
	//OUT.fbdd_noiserd = 2;
	//OUT.cfaline = 1;
	//OUT.linenoise = 0.02;*/

	/*OUT.cfa_clean = 1;
	OUT.cclean =0.05;*/

	//OUT.cfa_green = 0;
	//OUT.green_thresh = 0.1;



	int  i, ret, verbose = 0, output_thumbs = 0;
	LibRawUnpackResult r;
	r.Error = 0;
	//char outfn[1024], thumbfn[1024];
	//RawProcessor.imgdata.sizes


	// Определим переменные для удобного доступа к полям RawProcessor


	//OUT.output_tiff = 1; // Выводить будем TIFF
	//OUT.output_bps = 16;
	//OUT.gamm[0] = OUT.gamm[1] = OUT.no_auto_bright = 1;

	// Откроем файл
	std::string narrow(something->Begin(), something->End());

	ret = RawProcessor.open_file(narrow.c_str());
	if (ret != LIBRAW_SUCCESS){
		RawProcessor.recycle();
		r.Error = ret;

		return r;
	}

	//if ((ret = RawProcessor.open_buffer((void*)(something->Data), something->Length)) != LIBRAW_SUCCESS)
	//{*/
	//fprintf(stderr, "Cannot open %s: %s\n", av[i], libraw_strerror(ret));

	// recycle() нужен только если мы хотим освободить ресурсы прямо сейчас.
	// Если мы обрабатываем файлы в цикле, то следующий open_file() 
	// тоже вызовет recycle. Если случилась фатальная ошибка, то recycle()
	// уже вызван (вреда от повторного вызова тоже нет)

	//	RawProcessor.recycle();
	//	//goto end;
	//}
	//something = null;
	//delete[] something;
	// Распакуем изображение
	if ((ret = RawProcessor.unpack()) != LIBRAW_SUCCESS)
	{
		if (LIBRAW_FATAL_ERROR(ret)){
			RawProcessor.recycle();
			r.Error = ret;

			return r;
		}

	}

	int iii = RawProcessor.imgdata.process_warnings;

	r.Width = RawProcessor.imgdata.sizes.width;
	r.Height = RawProcessor.imgdata.sizes.height;

	return r;

}


