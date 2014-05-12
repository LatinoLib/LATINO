LATINO
======

Get It and Set It Up
--------------------

1. Clone LATINO from the GIT repository into, for example, C:\Work\Latino:

 ```
 C:\Work>git clone http://source.ijs.si/mgrcar/latino.git Latino
 ```

2. If you plan to use any of the SVM models (SvmBinaryClassifier, SvmMulticlassClassifier) and/or the least-squares regression model (LSqrModel), you need to add the path C:\Work\Latino\Bin32 to the environmental variable PATH. On a 64-bit OS, you need to add C:\Work\Latino\Bin64 instead.

Test It
-------

1. Start Visual Studio.
2. Create a new console-mode project.
3. Add Latino.csproj (C:\Work\Latino\Latino.csproj) to the solution. This project file was created in Visual Studio 2008. If you are using a later version, VS will automatically upgrade it.
4. Reference LATINO from the created project (References / Add Reference).
5. Copy-paste the following code into Program.cs:

 ```
 using System;
 using Latino;
 
 namespace LatinoTest
 {
        class Program
        {
            static void Main(string[] args)
            {
                SparseVector<char> v = new SparseVector<char>("Hello World!".ToCharArray());
                Console.WriteLine(v);
            }
        }
 }
 ```

6. Start the program.

Building Dependencies
---------------------

If you plan to use any of the SVM models (SvmBinaryClassifier, SvmMulticlassClassifier) and/or the least-squares regression model (LSqrModel), you need two unmanaged libraries, SvmLightLib and LSqrLib, respectively. These libraries come precompiled with LATINO but in case you want to make changes, you can build them yourself by following the instructions below.

### SvmLightLib

#### Get It

Clone [SvmLightLib](http://source.ijs.si/mgrcar/svmlightlib.git) from the GIT repository into, for example, C:\Work\SvmLightLib:

```
C:\Work>git clone http://source.ijs.si/mgrcar/svmlightlib.git SvmLightLib
```

#### Compile and Deploy It

1. Start Visual Studio.
2. Open SvmLightLib.sln (C:\Work\SvmLightLib\SvmLightLib.sln). This solution file and the referenced project files were created in Visual Studio 2008. If you are using a later version, VS will automatically convert these files.
3. Set SvmLightLib as the start-up project.
4. Build the library:
  * If you need the 32-bit binaries, build the configurations Debug/x86 and Release/x86. The DLL files, SvmLightLibDebug.dll and SvmLightLib.dll, will appear in the folder bin\x86 (C:\Work\SvmLightLib\bin\x86).
  * If you need the 64-bit binaries, build the configurations Debug/x64 and Release/x64. The DLL files, SvmLightLibDebug.dll and SvmLightLib.dll, will appear in the folder bin\x64 (C:\Work\SvmLightLib\bin\x64).
5. Copy the DLL files into the LATINO Bin folder (C:\Work\Latino\Bin32 or C:\Work\Latino\Bin64). Make sure that you have added the LATINO Bin folder to the environmental variable PATH.

### LSqrLib

#### Get It

Clone [LSqrLib](http://source.ijs.si/mgrcar/lsqrlib.git) from the GIT repository into, for example, C:\Work\LSqrLib:

```
C:\Work>git clone http://source.ijs.si/mgrcar/lsqrlib.git LSqrLib
```

#### Compile and Deploy It

1. Start Visual Studio.
2. Open LSqrDll.sln (C:\Work\LSqrLib\LSqrDll\Src\LSqrDll.sln). This solution file and the referenced project files were created in Visual Studio 2008. If you are using a later version, VS will automatically convert these files.
3. Build the library:
  * If you need the 32-bit binaries, build the configurations Debug/x86 and Release/x86. The DLL files, LSqrDebug.dll and LSqr.dll, will appear in the folder Bin\x86 (C:\Work\LSqrLib\LSqrDll\Bin\x86).
  * If you need the 64-bit binaries, build the configurations Debug/x64 and Release/x64. The DLL files, LSqrDebug.dll and LSqr.dll, will appear in the folder Bin\x64 (C:\Work\LSqrLib\LSqrDll\Bin\x64).
5. Copy the DLL files into the LATINO Bin folder (C:\Work\Latino\Bin32 or C:\Work\Latino\Bin64). Make sure that you have added the LATINO Bin folder to the environmental variable PATH.

License
-------

Most of LATINO is under [the MIT license](http://opensource.org/licenses/MIT). However, certain parts fall under other licenses. See [LICENSE.txt](http://source.ijs.si/mgrcar/latino/blob/master/LICENSE.txt) for more details.