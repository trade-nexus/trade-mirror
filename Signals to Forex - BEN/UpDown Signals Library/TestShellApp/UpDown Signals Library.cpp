#include "stdafx.h"
#include "Functions.h"
#include <stdexcept>
#include <stdlib.h>
#include <time.h>
#include <iostream>
#include <fstream>

using namespace std;

namespace UpDownSignalsLibrary
{
	string getTime()
	{
		time_t rawtime;
		time (&rawtime);
		return ctime(&rawtime);
	}

	bool SpawnClientTerminal(int hwnd, char* terminalName)
	{
		string path, logPath, mode;

		path = getenv("APPDATA");

		string logFilePath = path + "\\UpDown Signals\\logs";

		path = path + "\\UpDown Signals\\" + terminalName + "\\" + terminalName + ".exe";
		
		logFilePath = logFilePath + "\\Client Terminal.txt";
		ofstream logFile;
		try
		{
			logFile.open(logFilePath, ios::app);

			logFile << getTime() << "Order Pad Path = " << path << endl;
			logFile << getTime() << "Log File Path = " << logFilePath << endl;
					
			ShellExecuteA((HWND)hwnd, "open", LPCSTR(path.c_str()), NULL, NULL, SW_SHOWNORMAL);

			logFile << getTime() << "Order Pad executed successfully"<< endl;
			logFile.close();
			return true;
		}
		catch(exception exception)
		{
			logFile << getTime() << "Exception = " << exception.what() << endl;
			logFile.close();
			return false;
		}
	}

	
	bool WriteToFile(char *message, char* fileName, char* terminalName, int mode)
	{
		string path;
		
		path = getenv("APPDATA");
		path = path + "\\UpDown Signals\\";
		
		ofstream logFile, outputFile;
		string logPath = path + "logs\\File Writer Logs.txt";
		string outputFilePath = path + terminalName + "\\" + fileName;

		try
		{
			logFile.open(logPath, ios::app);
			if(mode == 0)
			{
				outputFile.open(outputFilePath, ios::app);
			}
			else
			{
				outputFile.open(outputFilePath, ios::trunc);
			}

			logFile << getTime() << "Output File Path = " << outputFilePath << endl;
			logFile << getTime() << "Log File Path = " << logPath << endl;

			outputFile << message << endl;

			outputFile.close();
			logFile.close();
			return true;
		}
		catch(exception exception)
		{
			logFile << getTime() << "Exception = " << exception.what() << endl;
			outputFile.close();
			logFile.close();
			return false;
		}
	}

	char* ReadFromFile(char* fileName, char* terminalName)
	{
		string path;
		char* message;
		
		path = getenv("APPDATA");
		path = path + "\\UpDown Signals\\";
		
		ofstream logFile;
		ifstream inputFile;
		string logPath = path + "logs\\File Reader Logs.txt";
		string inputFilePath = path + terminalName + "\\" + fileName;

		try
		{
			logFile.open(logPath, ios::app);
			inputFile.open(inputFilePath, ios::in);

			logFile << getTime() << "Input File Path = " << inputFilePath << endl;
			logFile << getTime() << "Log File Path = " << logPath << endl;

			if (inputFile.is_open())
			{
				string tempString;
				logFile << getTime() << "File: " << fileName << " opened" << endl;
				getline(inputFile,tempString);
				logFile << getTime() << "Read Message is = " << tempString << " | Size = " << tempString.size() << "bytes" << endl;

				message = new char[tempString.size() + 1];
				message[tempString.size()] = 0;
				memcpy(message, tempString.c_str(), tempString.size());

				logFile << getTime() << "Converted Message is = " << message << endl;

				inputFile.close();
				logFile.close();

				return message;
			}
			else
			{
				logFile << getTime() << "Unable to open file" << endl;
				inputFile.close();
				logFile.close();
				return "-1";
			}
		}
		catch(exception exception)
		{
			logFile << getTime() << "Exception = " << exception.what() << endl;
			inputFile.close();
			logFile.close();
			return "-1";
		}
	}

	bool FileDeleteExternal(char* fileName, char* terminalName)
	{
		string path;
		
		path = getenv("APPDATA");
		path = path + "\\UpDown Signals\\";
		
		ofstream logFile, outputFile;
		string logPath = path + "logs\\File Delete Logs.txt";
		string outputFilePath = path + terminalName + "\\" + fileName;

		try
		{
			logFile.open(logPath, ios::app);

			logFile << getTime() << "Deleting File Path = " << outputFilePath << endl;
			logFile << getTime() << "Log File Path = " << logPath << endl;

			if(remove(outputFilePath.c_str()) != 0)
			{
				logFile << getTime() << "Error Deleting file" << endl;
				logFile.close();
				return false;
			}
			else
			{
				logFile << getTime() << "File successfully deleted" << endl;
				logFile.close();
				return true;
			}
		}
		catch(exception exception)
		{
			logFile << getTime() << "Exception = " << exception.what() << endl;
			logFile.close();
			return false;
		}
	}
}

