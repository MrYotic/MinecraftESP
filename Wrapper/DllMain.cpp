#include <Windows.h>

BOOL __stdcall DllMain(HINSTANCE handle, DWORD reason, LPVOID reserved)
{
	switch (reason)
	{
		case DLL_PROCESS_ATTACH:
		{
			HMODULE module = GetModuleHandleW(L"MinecraftESP");
			void* loadFuncPtr = GetProcAddress(module, "Load");
			((void(*)(void))loadFuncPtr)();
		}
		case DLL_PROCESS_DETACH:
		{
			HMODULE module = GetModuleHandleW(L"MinecraftESP");
			void* unloadFuncPtr = GetProcAddress(module, "Unload");
			((void(*)(void))unloadFuncPtr)();
		}
		break;
	}
	return TRUE;
}