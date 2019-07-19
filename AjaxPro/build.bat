set NET11="%WINDIR%\Microsoft.NET\Framework\v1.1.4322"
set NET20="%WINDIR%\Microsoft.NET\Framework\v2.0.50727"
set ZIP=C:\Program Files\7-Zip\
set ARG=
set DEFINE=
set VERSION=7.7.31.1

call build_2.0.bat
call build_json.bat

cd Release

"%ZIP%7z.exe" a -tZIP "%VERSION%(no strong name)_DLL.zip" *.dll
copy "%VERSION%(no strong name)_DLL.zip" "..\%VERSION%(no strong name)_DLL.zip"

pause

del *.dll
cd ..


set DEFINE=STRONGNAME;

call build_2.0.bat
call build_json.bat


REM cd ..
REN cd AjaxProTemplate
REM call createpackage.bat
REM cd ..
REM cd AjaxPro

del "%VERSION%_DLL.zip"

REM "%ZIP%7za.exe" a -tZIP "%VERSION%_DLL.zip" AjaxProVSTemplate.vsi
"%ZIP%7z.exe" a -tZIP "%VERSION%_DLL.zip" web.config

cd Release

"%ZIP%7z.exe" a -tZIP "..\%VERSION%_DLL.zip" *.dll

cd ..

pause