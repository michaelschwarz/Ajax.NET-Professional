# Ajax.NET-Professional

Ajax.NET Professional (AjaxPro) is one of the first AJAX frameworks available for Microsoft ASP.NET and is working with .NET 1.1 and 2.0.

The framework will create proxy classes that are used on client-side JavaScript to invoke methods on the web server with full data type support working on all common web browsers including mobile devices. Return your own classes, structures, DataSets, enums,... as you are doing directly in .NET.

# A quick guide how to start

- Download the latest Ajax.NET Professional
- Add a reference to the AjaxPro.2.dll (for the .NET 1.1 Framework use AjaxPro.dll)
- Add following lines to your web.config

```XML
<?xml version="1.0" encoding="utf-8" ?>
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
      AjaxPro.Utility.RegisterTypeForAjax(typeof(_Default));
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
