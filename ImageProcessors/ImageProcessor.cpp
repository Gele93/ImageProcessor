#include "pch.h"
#include "ImageProcessor.h"
#include <cstring>
#include <opencv2/opencv.hpp>
#include <thread>
#include <vector>


void ApplyGaussianBlur(unsigned char* inputImage, unsigned char* outputImage, int width, int heigth) {

	cv::Mat inputMat(heigth, width, CV_8UC3, inputImage);
	cv::Mat outputMat(heigth, width, CV_8UC3, outputImage);

	int nThreads = std::thread::hardware_concurrency();
	if (nThreads == 0) nThreads = 1;

	int scopeHeight = heigth / nThreads;

	std::vector<std::thread> threads;

	for (int i = 0; i < nThreads; i++)
	{
		int startRow = i * scopeHeight;
		int endRow = (i == nThreads - 1) ? heigth : startRow + scopeHeight;

		threads.emplace_back([=, &inputMat, &outputMat]() {
			cv::Rect roi(0, startRow, width, endRow - startRow);
			cv::Mat inROI = inputMat(roi);
			cv::Mat outROI = outputMat(roi);
			cv::GaussianBlur(inROI, outROI, cv::Size(101, 101), 0);
			});
	}

	for (auto& t : threads)
		t.join();
}
