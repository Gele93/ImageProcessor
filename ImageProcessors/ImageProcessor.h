#pragma once

#ifdef IMAGEPROCESSORNATIVE_EXPORTS
#define IMAGEPROCESSORNATIVE_API __declspec(dllexport)
#else
#define IMAGEPROCESSORNATIVE_API __declspec(dllimport)
#endif

extern "C" IMAGEPROCESSORNATIVE_API
void ApplyGaussianBlur(unsigned char* inputImage, unsigned char* outputImage, int width, int height);
