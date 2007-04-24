REM del "release\AjaxPro.2(including webevent).dll"
REM /define:"TRACE;NET20;NET20external;WEBEVENT;%DEFINE%"
REM ren "release\AjaxPro.2.dll" "release\AjaxPro.2(including webevent).dll"

del release\AjaxPro.2.dll
"%NET20%\csc.exe" %ARG% /out:"release\AjaxPro.2.dll" /target:library /define:"TRACE;NET20;NET20external;%DEFINE%" /r:"System.dll" /r:"System.Data.dll" /r:"System.Drawing.dll" /r:"System.Web.dll" /r:"System.Web.Services.dll" /r:"System.Xml.dll" /r:"System.Configuration.dll" "AssemblyInfo.cs" "Attributes\*.cs" "Configuration\*.cs" "Handler\*.cs" "Handler\AjaxProcessors\*.cs" "Handler\Security\*.cs" "Interfaces\*.cs" "JSON\Converters\*.cs" "JSON\Interfaces\*.cs" "JSON\*.cs" "JSON\JavaScriptObjects\*.cs" "Managment\*.cs" "Security\*.cs" "Services\*.cs" "Utilities\*.cs" /res:prototype.js,AjaxPro.2.prototype.js /res:core.js,AjaxPro.2.core.js /res:ms.js,AjaxPro.2.ms.js
