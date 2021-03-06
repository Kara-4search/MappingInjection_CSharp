# MappingInjection_CSharp

Blog link: working on it

- Mapping-Injection: Just another Windows Process Injection.
- Mapping injection is a process injection technique that avoids the usage of common monitored syscall VirtualAllocEx and WriteProcessMemory.
- This can be achieved by using the Syscall MapViewOfFile2() and some preliminary steps to “prepare” the memory with the required shellcode.
- Works fine both on x64/x86.
- Supported OS: 
	* **Windows 10 / Windows Server 2016, version 1703 (build 10.0.15063) and above versions.**
- The function "MapViewOfFile2()", I could not find any definition of it even in the p/invoke website.
	* So I convert the [original version](https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-mapviewoffile2) to C#，
	* But, unluckily, that is not working.
	* The [page](https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-mapviewoffile2) show that "MapViewOfFile2()" is in kernel32.dll, but actually you'll get the error: cannot find the entry point of the function.
	* Looking at the definition of MapViewOfFile2() in the "memoryapi.h"  and I just noticed that it's just a wrapper for the function MapViewOfFileNuma2().
	* **The function MapViewOfFileNuma2() is imported from Kernelbase.dll or Api-ms-win-core-memory-l1-1-5.dll.**
	* I have no idea the differences of "MapViewOfFileNuma2" between these two DLLs, feel free to tell me~
	* In this project, I use Kernelbase.dll.
- And here is the definition of MapViewOfFileNuma2
```
// MapViewOfFile2 is just an inline function that calls MapViewOfFileNuma2 with
WINBASEAPI PVOID WINAPI MapViewOfFileNuma2(HANDLE aFileMapping, HANDLE aProcess,
                                           ULONG64 aOffset, PVOID aBaseAddress,
                                           SIZE_T aViewSize,
                                           ULONG aAllocationType,
                                           ULONG aPageProtection,
                                           ULONG aPreferredNode);

```
- Comparing with MapViewOfFile2, you could see there are, indeed a little different - ULONG aPreferredNode.
- **Its preferred node set to NUMA_NO_PREFERRED_NODE and NUMA_NO_PREFERRED_NODE = 0xffffffff;**
- **The original shellcode is a messagebox - "Hello via syscall", but is not syscall actually~**
```
            /*   Messagebox shellcode   */
            byte[] buf1 = new byte[328] {
                 0xfc, 0x48, 0x81, 0xe4, 0xf0, 0xff, 0xff, 0xff, 0xe8, 0xd0, 0x00, 0x00,
                 0x00, 0x41, 0x51, 0x41, 0x50, 0x52, 0x51, 0x56, 0x48, 0x31, 0xd2, 0x65,
                 0x48, 0x8b, 0x52, 0x60, 0x3e, 0x48, 0x8b, 0x52, 0x18, 0x3e, 0x48, 0x8b,
                 0x52, 0x20, 0x3e, 0x48, 0x8b, 0x72, 0x50, 0x3e, 0x48, 0x0f, 0xb7, 0x4a,
                 0x4a, 0x4d, 0x31, 0xc9, 0x48, 0x31, 0xc0, 0xac, 0x3c, 0x61, 0x7c, 0x02,
                 0x2c, 0x20, 0x41, 0xc1, 0xc9, 0x0d, 0x41, 0x01, 0xc1, 0xe2, 0xed, 0x52,
                 0x41, 0x51, 0x3e, 0x48, 0x8b, 0x52, 0x20, 0x3e, 0x8b, 0x42, 0x3c, 0x48,
                 0x01, 0xd0, 0x3e, 0x8b, 0x80, 0x88, 0x00, 0x00, 0x00, 0x48, 0x85, 0xc0,
                 0x74, 0x6f, 0x48, 0x01, 0xd0, 0x50, 0x3e, 0x8b, 0x48, 0x18, 0x3e, 0x44,
                 0x8b, 0x40, 0x20, 0x49, 0x01, 0xd0, 0xe3, 0x5c, 0x48, 0xff, 0xc9, 0x3e,
                 0x41, 0x8b, 0x34, 0x88, 0x48, 0x01, 0xd6, 0x4d, 0x31, 0xc9, 0x48, 0x31,
                 0xc0, 0xac, 0x41, 0xc1, 0xc9, 0x0d, 0x41, 0x01, 0xc1, 0x38, 0xe0, 0x75,
                 0xf1, 0x3e, 0x4c, 0x03, 0x4c, 0x24, 0x08, 0x45, 0x39, 0xd1, 0x75, 0xd6,
                 0x58, 0x3e, 0x44, 0x8b, 0x40, 0x24, 0x49, 0x01, 0xd0, 0x66, 0x3e, 0x41,
                 0x8b, 0x0c, 0x48, 0x3e, 0x44, 0x8b, 0x40, 0x1c, 0x49, 0x01, 0xd0, 0x3e,
                 0x41, 0x8b, 0x04, 0x88, 0x48, 0x01, 0xd0, 0x41, 0x58, 0x41, 0x58, 0x5e,
                 0x59, 0x5a, 0x41, 0x58, 0x41, 0x59, 0x41, 0x5a, 0x48, 0x83, 0xec, 0x20,
                 0x41, 0x52, 0xff, 0xe0, 0x58, 0x41, 0x59, 0x5a, 0x3e, 0x48, 0x8b, 0x12,
                 0xe9, 0x49, 0xff, 0xff, 0xff, 0x5d, 0x49, 0xc7, 0xc1, 0x00, 0x00, 0x00,
                 0x00, 0x3e, 0x48, 0x8d, 0x95, 0x1a, 0x01, 0x00, 0x00, 0x3e, 0x4c, 0x8d,
                 0x85, 0x35, 0x01, 0x00, 0x00, 0x48, 0x31, 0xc9, 0x41, 0xba, 0x45, 0x83,
                 0x56, 0x07, 0xff, 0xd5, 0xbb, 0xe0, 0x1d, 0x2a, 0x0a, 0x41, 0xba, 0xa6,
                 0x95, 0xbd, 0x9d, 0xff, 0xd5, 0x48, 0x83, 0xc4, 0x28, 0x3c, 0x06, 0x7c,
                 0x0a, 0x80, 0xfb, 0xe0, 0x75, 0x05, 0xbb, 0x47, 0x13, 0x72, 0x6f, 0x6a,
                 0x00, 0x59, 0x41, 0x89, 0xda, 0xff, 0xd5, 0x48, 0x65, 0x6C, 0x6C, 0x6F,
                 0x20, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x20, 0x76, 0x69, 0x61, 0x20, 0x73,
                 0x79, 0x73, 0x63, 0x61, 0x6C, 0x6C, 0x00, 0x41, 0x50, 0x49, 0x20, 0x54,
                 0x65, 0x73, 0x74, 0x00
             };
```


## Usage
1. Replace the shellcode with your own.
	![avatar](https://raw.githubusercontent.com/Kara-4search/ProjectPics/main/MappingInject_shellcode.png)
2. Set the process name you want to inject
	* default name in the project is Powershell.
	![avatar](https://raw.githubusercontent.com/Kara-4search/ProjectPics/main/MappingInject_processname.png)
3. And the messagebox show up.
	![avatar](https://raw.githubusercontent.com/Kara-4search/ProjectPics/main/MappingInject_messagebox.png)
	
## TO-DO list
- Update with "Early Bird" - DONE
	* Base on my another project(https://github.com/Kara-4search/EarlyBirdInjection_CSharp)
	* All in "MappingEarlyBirdInjection.cs".


## Update history
- Update with "Early Bird" process injection - 20210830
- Fix bugs for [#issues1](https://github.com/Kara-4search/MappingInjection_CSharp/issues/1)(Both MappingEarlyBirdInjection and MappingInjection) - 20211120
	* Haven’t test that in X86 thougth


## Reference link:
	1. https://breakdev.org/defeating-antivirus-real-time-protection-from-the-inside/
	2. https://github.com/antonioCoco/Mapping-Injection
	3. https://hakin9.org/mapping-injection-just-another-windows-process-injection/
	4. https://idiotc4t.com/code-and-dll-process-injection/mapping-injection
	5. http://blog.leanote.com/post/snowming/a0366d1d01bf
	6. https://idiotc4t.com/defense-evasion/load-ntdll-too
	7. https://www.ired.team/offensive-security/code-injection-process-injection/ntcreatesection-+-ntmapviewofsection-code-injection
	8. http://pinvoke.net/default.aspx/kernel32/CreateFileMapping.html
	9. https://www.displayfusion.com/Discussions/View/converting-c-data-types-to-c/?ID=38db6001-45e5-41a3-ab39-8004450204b3
	10. https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-mapviewoffilenuma2
	11. https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-mapviewoffile2
	12. https://docs.microsoft.com/en-us/windows/win32/memory/memory-protection-constants
	13. http://pinvoke.net/default.aspx/kernel32.MapViewOfFile