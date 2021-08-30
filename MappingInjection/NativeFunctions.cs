using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MappingInjection.NativeStructs;

namespace MappingInjection
{
    class NativeFunctions
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpFileMappingAttributes,
            FileMapProtection flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow,
            [MarshalAs(UnmanagedType.LPStr)] string lpName);

        /*
        LPVOID MapViewOfFile(
          HANDLE hFileMappingObject,
          DWORD dwDesiredAccess,
          DWORD dwFileOffsetHigh,
          DWORD dwFileOffsetLow,
          SIZE_T dwNumberOfBytesToMap
        );
        */

        [DllImport("kernel32.dll")]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            FileMapAccessType dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            uint processAccess,
            bool bInheritHandle,
            int processId);

        /* convert the function down below to c# version
            PVOID MapViewOfFile2(
              HANDLE  FileMappingHandle,
              HANDLE  ProcessHandle,
              ULONG64 Offset,
              PVOID   BaseAddress,
              SIZE_T  ViewSize,
              ULONG   AllocationType,
              ULONG   PageProtection
            );
        */

        [DllImport("kernelbase.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFileNuma2(
            IntPtr FileMappingHandle,
            IntPtr ProcessHandle,
            UInt64 Offset,
            IntPtr BaseAddress,
            int ViewSize,
            UInt32 AllocationType,
            UInt32 PageProtection,
            UInt32 Numa);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes, 
            uint dwStackSize, 
            IntPtr lpStartAddress,
            IntPtr lpParameter, 
            uint dwCreationFlags, 
            out uint lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 QueueUserAPC(IntPtr pfnAPC, IntPtr hThread, UInt32 dwData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr hThread);
    }
}
