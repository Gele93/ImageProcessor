#include "pch.h"
#include "ImageProcessor.h"
#include <cstring>
#include <opencv2/opencv.hpp>
#include <thread>
#include <vector>


void ApplyGaussianBlur(unsigned char* inputImage, unsigned char* outputImage, int width, int height, int blurStrength) {

	cv::Mat inputBGR(height, width, CV_8UC4, inputImage);
	cv::Mat outputBGR(height, width, CV_8UC4, outputImage);

	int nThreads = std::thread::hardware_concurrency();
	if (nThreads == 0) nThreads = 1;

	int scopeHeight = height / nThreads;

	std::vector<std::thread> threads;

	for (int i = 0; i < nThreads; i++)
	{
		int startRow = i * scopeHeight;
		int endRow = (i == nThreads - 1) ? height : startRow + scopeHeight;

		threads.emplace_back([=, &inputBGR, &outputBGR]() {
			cv::Rect roi(0, startRow, width, endRow - startRow);
			cv::Mat inROI = inputBGR(roi);
			cv::Mat outROI = outputBGR(roi);
			cv::GaussianBlur(inROI, outROI, cv::Size(blurStrength, blurStrength), 0);
			});
	}

	for (auto& t : threads)
		t.join();
}
