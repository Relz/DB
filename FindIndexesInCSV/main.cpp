#include "stdafx.h"
#include "FileValidation.h"

using namespace std;

void PrintError(const string & errorMessage)
{
	cerr << "Error: " << errorMessage << "\n";
}

void PrintVector(const vector<string> & vect)
{
	copy(vect.begin(), vect.end(), ostream_iterator<string>(cout, ", "));
	cout << "\n";
}

bool SplitStringToVector(const string & str, vector<string> & vect, char delimiter)
{
	vect.clear();
	string word;
	for (size_t i = 0; i < str.length(); ++i)
	{
		if (str[i] == delimiter)
		{
			vect.push_back(word);
			word.clear();
		}
		else
		{
			word += str[i];
		}
	}
	if (!word.empty())
	{
		vect.push_back(word);
	}
	return !vect.empty();
}

bool GetAuthor(string & author)
{
	bool result = false;
	cout << "Enter username: ";
	if (getline(cin, author))
	{
		result = true;
	}
	return result;
}

bool GetColumnPosInTableHeader(const vector<string> & tableHeader, const string & columnName, size_t & columnPos)
{
	bool result = false;
	for (size_t i = 0; i < tableHeader.size(); ++i)
	{
		if (tableHeader.at(i) == columnName)
		{
			result = true;
			columnPos = i;
		}
	}
	return result;
}

bool GetIdByAuthorInRow(const vector<string> & row, const string & author, size_t idPosInRow, size_t authorPosInRow, string & id)
{
	bool result = false;
	if (row.at(authorPosInRow) == author || row.at(authorPosInRow) == "\"" + author + "\"")
	{
		result = true;
		id = row.at(idPosInRow);
	}
	return result;
}

int main()
{
	string inputFileName = "input.csv";
	ifstream inputFile(inputFileName);

	string errorMessage;
	if (!IsValidInputFile(inputFileName, inputFile, errorMessage))
	{
		PrintError(errorMessage);
		return 1;
	}

	string line;
	getline(inputFile, line);

	vector<string> tableHeader;
	if (!SplitStringToVector(line, tableHeader, ','))
	{
		PrintError("Failed to parse table header");
		return 1;
	}

	size_t idPosInRow = 0;
	size_t authorPosInRow = 0;
	if (!GetColumnPosInTableHeader(tableHeader, "id", idPosInRow))
	{
		PrintError("There is no \"id\" column in table");
		return 1;
	}
	if (!GetColumnPosInTableHeader(tableHeader, "author", authorPosInRow))
	{
		PrintError("There is no \"author\" column in table");
		return 1;
	}

	vector<string> indexes;
	string author;
	while (GetAuthor(author))
	{
		indexes.clear();
		bool found = false;
		vector<string> row;
		while (getline(inputFile, line))
		{
			if (!SplitStringToVector(line, row, ','))
			{
				continue;
			}
			string id;
			if (GetIdByAuthorInRow(row, author, idPosInRow, authorPosInRow, id))
			{
				found = true;
				indexes.push_back(id);
			}
		}
		if (found)
		{
			sort(indexes.begin(), indexes.end());
			cout << "Record id: ";
			PrintVector(indexes);
		}
		else
		{
			cout << "Author not found\n";
		}
		inputFile.clear();
		inputFile.seekg(0, ios::beg);
		getline(inputFile, line);
	}
	return 0;
}