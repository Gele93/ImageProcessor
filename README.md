# Image Processor Documentation

## Project Overview
**Name:** Image Processor  

**Description:**  
An ASP.NET Core REST API exposing endpoints for applying Gaussian Blur on inputed .png / .jpg images.

##  1. Operation flow:
  ### 1.1 ImageController:
  - **ApplyGaussianBlurToBase64Image:** awaits image as Base64 -> calls needed services -> returns FileStremResult
  - **ApplyGaussianBlurToUploadedImage:** awaits uploaded file(image) -> calls needed services -> returns FileStremResult

  ### 1.2 Utils:
  -	Handles content type validation

  ### 1.3 ImageConverter:
  - **GetRawRgbBytes:** awaits System.Drawing.Image -> returns raw rgb byte array using 32byte format
  - **EncodeRgbBytes:** awaits raw rgb byte array -> returns jpg/png encoded binary byte array
  - **ConvertImageToBytes:** awaits IFormFile -> returns byte array

  ### 1.4 ImageModifier:
  - **UseGaussianBlur:** awaits encoded binary byte array -> calls *GetRawRgbBytes* -> runs *ApplyGaussianBlur* -> returns raw rgb byte array

  ### 1.5 ExternalServices:
  - **ApplyGaussianBlur:** calls ApplyGaussianBlur from the native module

  ### 1.6 Native Module:
  - **ApplyGaussianBlur:** 
	- gets pointer to input 32byte RGBA array & output array (same sized as input array) ->
	- the C# code created BGRA format -> converts to RGBA
	- creates proper Mats for input & output
	- calculates Threads and Scope of Threads
	- iterate on threads -> creates ROIs of scope -> Blur the current ROI -> add blured data to temporary RGBA array
	- converts temporary RGBA array to BGRA array -> copy temporary BGRA to output array
 
## 2. Structure
- **ImageProcessor/:** ASP.NET project
- **ImageProcessors/:** C++ modules

- *ImageProcessors.dll* included in the C# project root folder
- *opencv_world4110d.dll* needs to be added to C# project root folder !

## 3. Run
```
- git clone https://github.com/Gele93/ImageProcessor.git
- cd ImageProcessor
- dotnet restore
- install *opencv 4.11.0* (https://opencv.org/releases/)
- copy *opencv_world4110d.dll* to *ImageProcessor/*
- build & run ImageProcessor.sln
- use *swagger* at https://localhost:7197/swagger/index.html
```

## 4. Contact
Developer: **[Gelecsák Tamás]**  
Email: **[gelecsak.tamas@gmail.com]**  
LinkedIn: **[https://www.linkedin.com/in/tamasgelecsak]**
