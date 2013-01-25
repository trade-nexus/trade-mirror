#include "stdafx.h"
#include "Functions.h"
#include <stdexcept>
#include <stdlib.h>
#include <time.h>
#include <iostream>
#include <fstream>

using namespace std;

namespace CommunicationLibrary
{
	string getTime()
	{
		time_t rawtime;
		time (&rawtime);
		return ctime(&rawtime);
	}

	bool SendToSocket(char *orderInfo)
	{
		string a;
		long answer;
		WSAData wsaData;
		WORD DLLVERSION;
		DLLVERSION = MAKEWORD(0,1);
		answer = WSAStartup(DLLVERSION, &wsaData);

		SOCKADDR_IN addr;
		int addrLen = sizeof(addr);

		string IP, port, path;
		
		IP = "127.0.0.1";
		port = "6666";

		//path = getenv("USERPROFILE");
		path = getenv("APPDATA");
		
		ofstream logFile;
		string logPath = path + "\\AutoFX Profits\\logs\\SocketWriterLogs.txt";
		logFile.open(logPath, ios::app);
				
		logFile << getTime()  << "IP = " << IP << endl;
		logFile << getTime()  << "Port = " << port << endl;
				
		logFile << getTime()  << "Entering Connection Process\n";
			
		unsigned short sPort = (unsigned short) strtoul(port.c_str(), NULL, 0);

		SOCKET sConnect;
		sConnect = socket(AF_INET, SOCK_STREAM, NULL);
		addr.sin_addr.s_addr = inet_addr(IP.c_str());
		addr.sin_family = AF_INET;
		addr.sin_port = htons(sPort);
			
		answer = connect(sConnect, (SOCKADDR*)&addr, sizeof(addr));
		if(answer == 0)
		{
			logFile << getTime()  << "Connected to socket\n";
			logFile << getTime()  << "Message =" << orderInfo << endl;
			try
			{
				logFile << getTime()  << "Entered TRY block" << endl;
				send(sConnect, orderInfo, strlen(orderInfo) , NULL);
				logFile << getTime()  << "DATA SENT TO SOCKET" << endl;
				closesocket(sConnect);
				logFile << getTime()  << "Socket Closed" << endl;
					
			}
			catch(exception e)
			{
				logFile << getTime()  << "Exception....\n" << e.what() << endl;	
			}

			logFile << getTime()  << "Going to return TRUE" << endl;
			logFile.close();
			return true;
			logFile << getTime()  << "RETURNED TRUE. NOW WHAT????" << endl;
		}
		else
		{
			logFile << getTime()  << "Connection to socket Failed\n";
			return false;
		}
	}

	//bool SpawnClientTerminal(int hwnd, char *terminalPath, char* runningMode, char* terminalName)
	bool SpawnClientTerminal(int hwnd, char* terminalName)
	{
		string path, logPath, mode;
		//path = terminalPath;
		//mode = runningMode;

		path = getenv("APPDATA");

		//string logFilePath = getenv("USERPROFILE");
		string logFilePath = path + "\\AutoFX Profits\\logs";

		path = path + "\\AutoFX Profits\\" + terminalName + "\\" + terminalName + ".exe";
		
		logFilePath = logFilePath + "\\ClientTerminal.txt";
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
		path = path + "\\AutoFX Profits\\";
		
		ofstream logFile, outputFile;
		string logPath = path + "logs\\FileWriterLogs.txt";
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
		path = path + "\\AutoFX Profits\\";
		
		ofstream logFile;
		ifstream inputFile;
		string logPath = path + "logs\\FileReaderLogs.txt";
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
		path = path + "\\AutoFX Profits\\";
		
		ofstream logFile, outputFile;
		string logPath = path + "logs\\FileDeleteLogs.txt";
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

