cmake_minimum_required(VERSION 3.6)
project(FindIndexesInCSV)

set(CMAKE_CXX_STANDARD 11)

set(SOURCE_FILES main.cpp FileValidation.cpp FileValidation.h stdafx.cpp stdafx.h)
add_executable(FindIndexesInCSV ${SOURCE_FILES})