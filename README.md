![License](https://img.shields.io/github/license/solsnicethings/ShieldOfFaith?color=red) ![GitHub Stars](https://img.shields.io/github/stars/solsnicethings/ShieldOfFaith?color=yellow)

## Shield of Faith

Windows program for making the computer monitor darker, letting the user update the specifics from an easily accessible control panel.

Two methods supported:

* In the main process, show an always-on-top window with translucency, filling one monitor which therefore appears darker.
* Run ShaderGlass as a separate process. ShaderGlass is a different project available on Github that allows rerendering the desktop in very powerful ways using custom .slang shaders.

The ShaderGlass source has been forked and modified to achieve integration with Shield of Faith.

The initial ShaderGlass code used, and Shield of Faith, are both made available under the GNU GPL v3 license.

### Essential references

Check out [Online Manual](https://mausimus.github.io/ShaderGlass/MANUAL.html) for details about ShaderGlass.

<br/>

### Download

https://www.patreon.com/posts/shield-of-faith-152427997

<br/>

### Requirements

Requirements from ShaderGlass:

* __Windows 10, version 2004__ (build 19041) or __Windows 11__
  * will work on version 1903 but in limited capacity (no Desktop Glass mode)
  * Windows 11 allows the __removal of yellow border__ (see [FAQ](FAQ.md#windows-10) for tips on avoiding it on Windows 10)
* DirectX 11-capable GPU

Requirements from Shield of Faith:

64-bits windows platform with .NET 4.8

<br/>

### Code

Shader glass is built using Visual Studio 2022 using ISO C++ 20, Windows SDK 10.0.26100, Windows Capture API and DirectX 11.

Shield of Faith was rebuilt with a clone/branch of that project as a starter solution in Visual Studio 2022 Community Edition.

ShaderGlass includes a limited implementation of RetroArch shader back-end using DirectX 11.
[ShaderGen](ShaderGen) is a command-line tool for converting Slang shaders 
into .h files which can be precompiled in ShaderGlass. The conversion process relies on:
1. [glslang](https://github.com/KhronosGroup/glslang) for converting Slang/GLSL shaders to SPIR-V
2. [SPIR-V cross-compiler](https://github.com/KhronosGroup/SPIRV-Cross) for converting those to HLSL (DX11 format)
3. [Direct3D Shader Compiler (fxc.exe)](https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk/) for pre-compiling into bytecode

<br/>

### Notes for programming and building

Shield of Faith was forked from ShaderGlass without any particular understanding of how the existing project was configured. (And I, no master of git, eventually started a fresh repository from file copy, because I made a mess of every template or fork based approach, so this repository has no technical connection to its origins.)

The .NET 4.8 project has a pre-build event using make-catalog.cmd to launch make-catalog.ps1 with -ExecutionPolicy RemoteSigned to create the embedded file Catalog.zip.

ShaderIPCLib is referenced both by a pure C++ project (ShaderGlass) and by a C++/CLR project (ShaderIPCforCLRControl). In DEBUG this builds fine for me. In release (x64 explicitly chosen) I have to switch the "Runtime library" project property under C/C++ "Code Generation" so it is multithreaded (/MT) for ShaderGlass and Multithreaded DLL (/MD) for Sol.ShieldOfFaith.

The Projects added by me are:
ShaderIPCLib, ShaderIPCforCLRControl, Sol.ShieldOfFaith.
The ShaderGlass project is modified by me to make use of ShaderIPCLib and integrate with Sol.ShieldOfFaith.

This modified version of ShaderGlass will only perform differently when launched from a process that passes the -ipc parameter and proceeds to communicate as designed in ShaderIPCLib.

The combination of Shield of Faith and ShaderGlass into a single bundle has not been automated. Doing so manually consists of:

1. Putting ShaderGlass.exe in the same folder as the Shield of Faith build output.
2. Renaming ShaderGlass.exe to ShaderGlassForShieldOfFaith.exe

<br/>

### Notices

* Shield of Faith application is provided under [GNU General Public License v3.0](LICENSE)

* ShaderGlass application is provided under [GNU General Public License v3.0](LICENSE)

* ShaderGlass includes precompiled shaders from [libretro/RetroArch shader repository](https://github.com/libretro/slang-shaders).
Please refer to copyright notes within shader code for detailed copyright and license information about each shader.

* ShaderGlass App icon courtesy of Icons-Land

* Shield of Faith Icon found at https://icon-icons.com/icon/insurance-protection-safety-shield/108037 , released by Laura Reen under a CC BY 4.0 license.
