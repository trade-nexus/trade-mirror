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

		path = getenv("USERPROFILE");
		
		ofstream logFile;
		string logPath = path + "\\Log_TradeSender_DLL.txt";
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

	bool SpawnClientTerminal(int hwnd, char *terminalPath, char* runningMode, char* terminalName)
	{
		string path, logPath, mode;
		path = terminalPath;
		mode = runningMode;

		string logFilePath = getenv("USERPROFILE");

		if(mode == "LIVE")
		{
			path = path + "\\experts\\files\\" + terminalName + ".exe";
		}
		else if(mode == "TEST")
		{
			path = path + "\\tester\\files\\" + terminalName + ".exe";
		}
		else
		{
			return false;
		}

		
		logFilePath = logFilePath + "\\LogClientTerminal.txt";
		ofstream logFile;
		try
		{
			logFile.open(logFilePath, ios::app);

			logFile << getTime() << "Terminal Path = " << terminalPath << endl;
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
}

