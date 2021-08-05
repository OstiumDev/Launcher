/*
 * Ostium Launcher, Injects a DLL into another program
 * Copyright (C) 2021 OstiumDev
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static OstiumLauncher.Win32;

namespace OstiumLauncher
{
    public static class Injector
    {
        public static bool InjectDllIntoProcess(string processName, string dllPath, out int errorCode)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                errorCode = Marshal.GetLastWin32Error();
                return false;
            }

            return InjectDllIntoProcess(processes[0].Id, dllPath, out errorCode);
        }

        public static bool InjectDllIntoProcess(int processId, string dllPath, out int errorCode)
        {
            errorCode = 0;
            // obtain a handle to the process
            nint procHandle = OpenProcess(ProcessAccessRight.PROCESS_CREATE_THREAD | ProcessAccessRight.PROCESS_VM_OPERATION | ProcessAccessRight.PROCESS_VM_WRITE, false, (uint)processId);
            if (procHandle == IntPtr.Zero)
            {
                goto SetErrorAndReturn;
            }

            // allocate memory to write in our dll path
            int dllPathLength = (dllPath.Length + 1) * sizeof(char);
            nint memAlloc = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)dllPathLength, AllocationType.MEM_COMMIT, ProtectionType.PAGE_READWRITE);
            if (memAlloc == IntPtr.Zero)
            {
                goto SetErrorAndReturn;
            }

            // write our dll path to memory
            if (!WriteProcessMemory(procHandle, memAlloc, Encoding.Default.GetBytes(dllPath), (uint)dllPathLength, out uint _))
            {
                goto SetErrorAndReturn;
            }

            // get the address of the function LoadLibraryA from kernel32.dll
            nint loadLib = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA");
            if (loadLib == IntPtr.Zero)
            {
                goto SetErrorAndReturn;
            }

            // create the thread
            nint threadHandle = CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLib, memAlloc, 0, out uint _);
            if (threadHandle == IntPtr.Zero)
            {
                goto SetErrorAndReturn;
            }

            return true;

        SetErrorAndReturn:
            errorCode = Marshal.GetLastWin32Error();
            return false;
        }
    }

    public static class Win32
    {
        private const string kDllName = "kernel32.dll";

        [DllImport(kDllName, SetLastError = true)]
        public static extern nint OpenProcess(ProcessAccessRight dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport(kDllName, SetLastError = true)]
        public static extern nint VirtualAllocEx(nint hProcess, nint lpAddress, uint dwSize, AllocationType flAllocationType, ProtectionType flProtect);

        [DllImport(kDllName, SetLastError = true)]
        public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesWritten);

        [DllImport(kDllName, SetLastError = true)]
        public static extern nint GetModuleHandleA(string lpModuleName);

        [DllImport(kDllName, SetLastError = true)]
        public static extern nint GetProcAddress(nint hModule, string lpProcName);

        [DllImport(kDllName, SetLastError = true)]
        public static extern nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, uint dwStackSize, nint lpStartAddress, nint lpParameter, uint dwCreationFlags, out uint lpThreadId);

        [Flags]
        public enum ProcessAccessRight : uint
        {
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_WRITE = 0x0020
        }

        [Flags]
        public enum AllocationType : uint
        {
            MEM_COMMIT = 0x00001000
        }

        [Flags]
        public enum ProtectionType : uint
        {
            PAGE_READWRITE = 0x04
        }
    }
}