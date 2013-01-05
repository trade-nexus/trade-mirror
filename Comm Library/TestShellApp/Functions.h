#pragma once
#pragma comment(lib, "Ws2_32.lib")

#include <sdkddkver.h>
#include <conio.h>
#include <stdio.h>
#include <windows.h>
#include <string>

namespace CommunicationLibrary
{
    class CommunicationLibrary
    {
    public:
		static __declspec(dllexport) bool SendToSocket(char*);
		static __declspec(dllexport) bool SpawnClientTerminal(int, char*, char*, char*);
    };
}