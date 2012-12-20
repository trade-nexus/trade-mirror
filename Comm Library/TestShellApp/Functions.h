#pragma once
#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "psapi.lib")

#include <sdkddkver.h>
#include <conio.h>
#include <stdio.h>
#include <windows.h>
#include <string>
#include <Psapi.h>

namespace CommunicationLibrary
{
    class CommunicationLibrary
    {
    public:
		static __declspec(dllexport) bool SendToSocket(char*);
    };
}