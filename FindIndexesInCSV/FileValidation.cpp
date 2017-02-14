#include "stdafx.h"
#include "FileValidation.h"

// Проверка существования файла
bool IsFileExists(std::ifstream &file)
{
	return file.good();
}

// Проверка файла на пустоту
bool IsFileEmpty(std::ifstream &file)
{
	return file.peek() == std::ifstream::traits_type::eof();
}

// Проверка файла ввода на существование и наличие в нём символов;
bool IsValidInputFile(const std::string & inFileName, std::ifstream &inputFile, std::string &errorMessage)
{
	if (!IsFileExists(inputFile))
	{
		errorMessage = "File \"" + inFileName + "\" does not exists";
		return false;
	}
	if (IsFileEmpty(inputFile))
	{
		errorMessage = "File \"" + inFileName + "\" is empty";
		return false;
	}
	return true;
}