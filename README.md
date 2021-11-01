# Ajax.NET Professional

Ajax.NET Professional (AjaxPro) is one of the first AJAX frameworks available for Microsoft ASP.NET.

The framework will create proxy JavaScript classes that are used on client-side to invoke methods on the web server with full data type support working on all common web browsers including mobile devices. Return your own classes, structures, DataSets, enums,... as you are doing directly in .NET.

# Quick Guide

- Download the latest Ajax.NET Professional
- Add a reference to the AjaxPro.2.dll (for the .NET 1.1 Framework use AjaxPro.dll)
- Add following lines to your web.config

```XML
<configuration>
  <system.web>
    <httpHandlers>
      <add verb="POST,GET" path="ajaxpro/*.ashx" type="AjaxPro.AjaxHandlerFactory, AjaxPro.2"/>
    </httpHandlers>
    [...]
  </system.web>
</configuration>
```

- Now, you have to mark your .NET methods with an AjaxMethod attribute

```C#
[AjaxPro.AjaxMethod]
public DateTime GetServerTime()
{
  return DateTime.Now;
}
```

- To use the .NET method on the client-side JavaScript you have to register the methods, this will be done to register a complete class to Ajax.NET

```C#
namespace MyDemo
{
  public class DefaultWebPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      AjaxPro.Utility.RegisterTypeForAjax(typeof(DefaultWebPage));
    }

    [AjaxPro.AjaxMethod]
    public static DateTime GetServerTime()
    {
      return DateTime.Now;
    }
  }
}
```

- If you start the web page two JavaScript includes are rendered to the HTML source
- To call a .NET method form the client-side JavaScript code you can use following syntax

```JavaScript
function getServerTime() {
  MyDemo.DefaultWebPage.GetServerTime(getServerTime_callback);  // asynchronous call
}

// This method will be called after the method has been executed
// and the result has been sent to the client.
function getServerTime_callback(res) {
  alert(res.value);
}
```

# Compiler Options

- `NET20` compiles .NET 2.0 assemblies AjaxPro.2.dll (otherwise original it was .NET 1.1, AjaxPro.dll)
- `JSONLIB` compiles JSON parser only (AjaxPro.JSON.2.dll or AjaxPro.JSON.dll)
- `NET20external` is setting the assembly name to AjaxPro.2.dll, compatibility
- `TRACE` is no longer used

# Security Settings

In web.config you can configure different security related settings.

One of the most important is to set a (Content-Security-Policy)[https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy] HTTP response header to ensure to trust only JavaScript and other resources that are coming from your web server or trusted locations. As (AjaxPro)[https://www.ajaxpro.info] is generating some JavaScript files on-the-fly you can set the JavaScript nonce in your web.config:

```XML
<configuration>
	[...]
	<ajaxNet>
		<ajaxSettings>
			<contentSecurityPolicy nonce="abcdefghijklmnopqrstuvwxyz" />
			[...]
		</ajaxSettings>
	</ajaxNet>
	<system.webServer>
		<httpProtocol>
			<customHeaders>
				<add name ="Content-Security-Policy" value ="frame-ancestors www.mydomain.com; script-src 'self' https://www.mydomain.com 'unsafe-eval' 'unsafe-hashes' 'nonce-abcdefghijklmnopqrstuvwxyz';" />
			</customHeaders>
		</httpProtocol>
	</system.webServer>
</configuration>
```