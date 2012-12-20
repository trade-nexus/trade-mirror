#include "stdafx.h"
#include "Functions.h"
#include <stdexcept>
#include <stdlib.h>
#include <time.h>
#include <iostream>
#include <fstream>
#include <Psapi.h>
#include <WinBase.h>

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
}

