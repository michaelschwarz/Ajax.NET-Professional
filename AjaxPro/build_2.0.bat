del release\AjaxPro.2.dll
del "release\AjaxPro.2(including webevent).dll"

"%NET20%\csc.exe" %ARG% /out:"release\AjaxPro.2.dll" /target:library /define:"TRACE;NET20;NET20external;WEBEVENT" /r:"System.dll" /r:"System.Data.dll" /r:"System.Drawing.dll" /r:"System.Web.dll" /r:"System.Web.Services.dll" /r:"System.Xml.dll" /r:"System.Configuration.dll" "AssemblyInfo.cs" "Attributes\AjaxNoTypeUsageAttribute.cs" "Attributes\AjaxCacheAttribute.cs" "Attributes\AjaxMethodAttribute.cs" "Attributes\AjaxNamespaceAttribute.cs" "Attributes\AjaxNonSerializableAttribute.cs" "Attributes\AjaxPropertyAttribute.cs" "Attributes\HttpSessionStateRequirement.cs" "Attributes\JavaScriptConverterAttribute.cs" "Attributes\__AjaxClassAttribute.cs" "Attributes\__AjaxEnumAttribute.cs" "Configuration\AjaxSettingsSectionHandler.cs" "Handler\AjaxRequestModule.cs" "Handler\HttpCompressionModule.cs" "Handler\AjaxAsyncHttpHandler.cs" "Handler\AjaxHandlerFactory.cs" "Handler\AjaxProcessors\IFrameProcessor.cs" "Handler\AjaxProcessors\XmlHttpRequestProcessor.cs" "Handler\AjaxProcHelper.cs" "Handler\AjaxSyncHttpHandler.cs" "Handler\ConverterJavaScriptHandler.cs" "Handler\EmbeddedJavaScriptHandler.cs" "Handler\Security\AjaxEncryption.cs" "Handler\SimpleResult.cs" "Handler\TypeJavaScriptHandler.cs" "Interfaces\IAjaxTokenProvider.cs" "Interfaces\IAjaxCryptProvider.cs" "Interfaces\IAjaxKeyProvider.cs" "Interfaces\IAjaxProcessor.cs" "Interfaces\IContextInitializer.cs" "JSON\Converters\BitmapConverter.cs" "JSON\Converters\DecimalConverter.cs" "JSON\Converters\EnumConverter.cs" "JSON\Converters\ExceptionConverter.cs" "JSON\Converters\GuidConverter.cs" "JSON\Converters\PrimitiveConverter.cs" "JSON\Converters\StringConverter.cs" "JSON\Converters\DataRowConverter.cs" "JSON\Converters\DataRowViewConverter.cs" "JSON\Converters\DataSetConverter.cs" "JSON\Converters\DataTableConverter.cs" "JSON\Converters\DataViewConverter.cs" "JSON\Converters\DateTimeConverter.cs" "JSON\Converters\HashtableConverter.cs" "JSON\Converters\HtmlControlConverter.cs" "JSON\Converters\JavaScriptObjectConverter.cs" "JSON\Converters\IDictionaryConverter.cs" "JSON\Converters\IEnumerableConverter.cs" "JSON\Converters\IListConverter.cs" "JSON\Converters\NameValueCollectionConverter.cs" "JSON\Converters\ProfileBaseConverter.cs" "JSON\Interfaces\IJavaScriptConverter.cs" "JSON\Interfaces\IJavaScriptObject.cs" "JSON\JavaScriptConverter.cs" "JSON\JavaScriptConverterCollection.cs" "JSON\JavaScriptDeserializer.cs" "JSON\JavaScriptObjects\JavaScriptArray.cs" "JSON\JavaScriptObjects\JavaScriptBoolean.cs" "JSON\JavaScriptObjects\JavaScriptNumber.cs" "JSON\JavaScriptObjects\JavaScriptObject.cs" "JSON\JavaScriptObjects\JavaScriptString.cs" "JSON\JavaScriptObjects\JavaScriptSource.cs" "JSON\JavaScriptSerializer.cs" "JSON\JavaScriptUtil.cs" "JSON\JSONParser.cs" "Managment\WebAjaxErrorEvent.cs" "Security\Authentication.cs" "Security\Decryptor.cs" "Security\DecryptTransformer.cs" "Security\Encryptor.cs" "Security\EncryptTransformer.cs" "Security\WebDecrypter.cs" "Security\WebEncrypter.cs" "Services\AuthenticationService.cs" "Services\CartService.cs" "Services\ChatService.cs" "Services\ProfileService.cs" "Utilities\AjaxSettings.cs" "Utilities\CacheInfo.cs" "Utilities\ClientMethod.cs" "Utilities\Constant.cs" "Utilities\MD5Helper.cs" "Utilities\Utility.cs" /res:prototype.js,AjaxPro.2.prototype.js /res:core.js,AjaxPro.2.core.js /res:ms.js,AjaxPro.2.ms.js
ren "release\AjaxPro.2.dll" "release\AjaxPro.2(including webevent).dll"

"%NET20%\csc.exe" %ARG% /out:"release\AjaxPro.2.dll" /target:library /define:"TRACE;NET20;NET20external" /r:"System.dll" /r:"System.Data.dll" /r:"System.Drawing.dll" /r:"System.Web.dll" /r:"System.Web.Services.dll" /r:"System.Xml.dll" /r:"System.Configuration.dll" "AssemblyInfo.cs" "Attributes\AjaxNoTypeUsageAttribute.cs" "Attributes\AjaxCacheAttribute.cs" "Attributes\AjaxMethodAttribute.cs" "Attributes\AjaxNamespaceAttribute.cs" "Attributes\AjaxNonSerializableAttribute.cs" "Attributes\AjaxPropertyAttribute.cs" "Attributes\HttpSessionStateRequirement.cs" "Attributes\JavaScriptConverterAttribute.cs" "Attributes\__AjaxClassAttribute.cs" "Attributes\__AjaxEnumAttribute.cs" "Configuration\AjaxSettingsSectionHandler.cs" "Handler\AjaxRequestModule.cs" "Handler\HttpCompressionModule.cs" "Handler\AjaxAsyncHttpHandler.cs" "Handler\AjaxHandlerFactory.cs" "Handler\AjaxProcessors\IFrameProcessor.cs" "Handler\AjaxProcessors\XmlHttpRequestProcessor.cs" "Handler\AjaxProcHelper.cs" "Handler\AjaxSyncHttpHandler.cs" "Handler\ConverterJavaScriptHandler.cs" "Handler\EmbeddedJavaScriptHandler.cs" "Handler\Security\AjaxEncryption.cs" "Handler\SimpleResult.cs" "Handler\TypeJavaScriptHandler.cs" "Interfaces\IAjaxTokenProvider.cs" "Interfaces\IAjaxCryptProvider.cs" "Interfaces\IAjaxKeyProvider.cs" "Interfaces\IAjaxProcessor.cs" "Interfaces\IContextInitializer.cs" "JSON\Converters\BitmapConverter.cs" "JSON\Converters\DecimalConverter.cs" "JSON\Converters\EnumConverter.cs" "JSON\Converters\ExceptionConverter.cs" "JSON\Converters\GuidConverter.cs" "JSON\Converters\PrimitiveConverter.cs" "JSON\Converters\StringConverter.cs" "JSON\Converters\DataRowConverter.cs" "JSON\Converters\DataRowViewConverter.cs" "JSON\Converters\DataSetConverter.cs" "JSON\Converters\DataTableConverter.cs" "JSON\Converters\DataViewConverter.cs" "JSON\Converters\DateTimeConverter.cs" "JSON\Converters\HashtableConverter.cs" "JSON\Converters\HtmlControlConverter.cs" "JSON\Converters\JavaScriptObjectConverter.cs" "JSON\Converters\IDictionaryConverter.cs" "JSON\Converters\IEnumerableConverter.cs" "JSON\Converters\IListConverter.cs" "JSON\Converters\NameValueCollectionConverter.cs" "JSON\Converters\ProfileBaseConverter.cs" "JSON\Interfaces\IJavaScriptConverter.cs" "JSON\Interfaces\IJavaScriptObject.cs" "JSON\JavaScriptConverter.cs" "JSON\JavaScriptConverterCollection.cs" "JSON\JavaScriptDeserializer.cs" "JSON\JavaScriptObjects\JavaScriptArray.cs" "JSON\JavaScriptObjects\JavaScriptBoolean.cs" "JSON\JavaScriptObjects\JavaScriptNumber.cs" "JSON\JavaScriptObjects\JavaScriptObject.cs" "JSON\JavaScriptObjects\JavaScriptString.cs" "JSON\JavaScriptObjects\JavaScriptSource.cs" "JSON\JavaScriptSerializer.cs" "JSON\JavaScriptUtil.cs" "JSON\JSONParser.cs" "Managment\WebAjaxErrorEvent.cs" "Security\Authentication.cs" "Security\Decryptor.cs" "Security\DecryptTransformer.cs" "Security\Encryptor.cs" "Security\EncryptTransformer.cs" "Security\WebDecrypter.cs" "Security\WebEncrypter.cs" "Services\AuthenticationService.cs" "Services\CartService.cs" "Services\ChatService.cs" "Services\ProfileService.cs" "Utilities\AjaxSettings.cs" "Utilities\CacheInfo.cs" "Utilities\ClientMethod.cs" "Utilities\Constant.cs" "Utilities\MD5Helper.cs" "Utilities\Utility.cs" /res:prototype.js,AjaxPro.2.prototype.js /res:core.js,AjaxPro.2.core.js /res:ms.js,AjaxPro.2.ms.js
