#include <Windows.h>
#include <string>
#include <iostream>
using namespace std;

BOOL __stdcall DllMain(HINSTANCE handle, DWORD reason, LPVOID reserved)
{
	if (reason != DLL_PROCESS_ATTACH)
		return TRUE;

	auto module = GetModuleHandleW(L"MinecraftESP");
	if (module == 0)
	{
		auto baseModule = GetModuleHandleW(L"Wrapper");
		TCHAR filenameBuff[MAX_PATH];
		GetModuleFileNameW(baseModule, filenameBuff, MAX_PATH);
		wstring filenameTemp(&filenameBuff[0]);
		string filename(filenameTemp.begin(), filenameTemp.end());

		auto directory = filename.substr(0, filename.find_last_of("/\\"));

		auto minecraftESPPath = directory.append("\\MinecraftESP.dll");
		module = LoadLibraryW(wstring(minecraftESPPath.begin(), minecraftESPPath.end()).c_str());
	}

	((void(*)(void))GetProcAddress(module, "Load"))();

	return TRUE;
} 