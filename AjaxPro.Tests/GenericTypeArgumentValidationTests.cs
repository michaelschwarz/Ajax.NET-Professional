using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AjaxPro.Tests
{
	public class Wrapper<T>
	{
		public T Value { get; set; }
	}

	public class Item
	{
		public string Name { get; set; }
	}

	public class DeniedItem
	{
		public string Name { get; set; }
	}

	/// <summary>
	/// A type name from the request can carry further type names as generic
	/// arguments or as the element type of an array. A member typed with a
	/// generic argument is deserialized as that argument, so each of these names
	/// has to pass the jsonDeserializationCustomTypes check, not only the outer one.
	///
	/// The type names are passed as the value type of a Hashtable entry:
	/// [[key, value, keyTypeName, valueTypeName]].
	/// </summary>
	[TestClass]
	public class GenericTypeArgumentValidationTests
	{
		private const string MissingAssembly = "AjaxProTestsAssemblyThatDoesNotExist";
		private const string Uri = "System.Uri, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

		private object m_Settings;
		private bool m_OriginalIsDisabled;
		private List<string> m_OriginalAllowed;
		private List<string> m_OriginalDenied;
		private List<string> m_ResolveRequests;

		[TestInitialize]
		public void Initialize()
		{
			// AjaxSettings is internal, the test changes the loaded settings by reflection.
			Type utility = typeof(JavaScriptDeserializer).Assembly.GetType("AjaxPro.Utility", true);
			m_Settings = utility.GetProperty("Settings", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);

			m_OriginalIsDisabled = (bool)GetSetting("IsCustomTypesDeserializationDisabled");
			m_OriginalAllowed = (List<string>)GetSetting("JsonDeserializationCustomTypesAllowed");
			m_OriginalDenied = (List<string>)GetSetting("JsonDeserializationCustomTypesDenied");

			m_ResolveRequests = new List<string>();
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}

		[TestCleanup]
		public void Cleanup()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;

			SetSetting("IsCustomTypesDeserializationDisabled", m_OriginalIsDisabled);
			SetSetting("JsonDeserializationCustomTypesAllowed", m_OriginalAllowed);
			SetSetting("JsonDeserializationCustomTypesDenied", m_OriginalDenied);
		}

		private object GetSetting(string name)
		{
			return m_Settings.GetType().GetProperty(name).GetValue(m_Settings, null);
		}

		private void SetSetting(string name, object value)
		{
			m_Settings.GetType().GetProperty(name).SetValue(m_Settings, value, null);
		}

		/// <summary>default="deny" with the test namespace on the allow list.</summary>
		private void DenyByDefaultAllowingTestNamespace()
		{
			SetSetting("IsCustomTypesDeserializationDisabled", true);
			SetSetting("JsonDeserializationCustomTypesAllowed", new List<string>(new string[] { "AjaxPro.Tests.*" }));
			SetSetting("JsonDeserializationCustomTypesDenied", new List<string>());
		}

		/// <summary>default="allow" with DeniedItem on the deny list.</summary>
		private void AllowByDefaultDenyingDeniedItem()
		{
			SetSetting("IsCustomTypesDeserializationDisabled", false);
			SetSetting("JsonDeserializationCustomTypesAllowed", new List<string>());
			SetSetting("JsonDeserializationCustomTypesDenied", new List<string>(new string[] { "AjaxPro.Tests.DeniedItem" }));
		}

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith(MissingAssembly, StringComparison.Ordinal))
				m_ResolveRequests.Add(args.Name);

			return null;
		}

		private static string WrapperOf(string argumentTypeName)
		{
			return "AjaxPro.Tests.Wrapper`1[[" + argumentTypeName + "]], AjaxPro.Tests";
		}

		private static object DeserializeHashtableValue(string valueJson, string valueTypeName)
		{
			string json = "[[\"key\"," + valueJson + ",\"System.String\",\"" + valueTypeName + "\"]]";

			Hashtable result = (Hashtable)JavaScriptDeserializer.DeserializeFromJson(json, typeof(Hashtable));

			return result["key"];
		}

		[TestMethod]
		public void DenyMode_UnlistedGenericArgument_IsRejectedBeforeTheTypeIsLoaded()
		{
			DenyByDefaultAllowingTestNamespace();

			Assert.ThrowsException<SecurityException>(
				() => DeserializeHashtableValue("{\"Value\":{}}", WrapperOf("Some.Unlisted.Type, " + MissingAssembly)));

			Assert.AreEqual(0, m_ResolveRequests.Count, "Type.GetType() was called with the unvalidated type name.");
		}

		[TestMethod]
		public void DenyMode_UnlistedGenericArgumentOfExistingType_IsRejected()
		{
			DenyByDefaultAllowingTestNamespace();

			Assert.ThrowsException<SecurityException>(
				() => DeserializeHashtableValue("{\"Value\":{}}", WrapperOf(Uri)));
		}

		[TestMethod]
		public void DenyMode_UnlistedGenericArgumentInTypeProperty_IsRejected()
		{
			DenyByDefaultAllowingTestNamespace();

			string json = "{\"__type\":\"" + WrapperOf(Uri) + "\",\"Value\":{}}";

			Assert.ThrowsException<SecurityException>(
				() => JavaScriptDeserializer.DeserializeFromJson(json, typeof(object)));
		}

		[TestMethod]
		public void DenyMode_AllowedGenericArgument_IsDeserialized()
		{
			DenyByDefaultAllowingTestNamespace();

			object value = DeserializeHashtableValue("{\"Value\":{\"Name\":\"test\"}}", WrapperOf("AjaxPro.Tests.Item, AjaxPro.Tests"));

			Assert.IsInstanceOfType(value, typeof(Wrapper<Item>));
			Assert.AreEqual("test", ((Wrapper<Item>)value).Value.Name);
		}

		[TestMethod]
		public void AllowMode_DeniedTypeAsGenericArgument_IsRejected()
		{
			AllowByDefaultDenyingDeniedItem();

			Assert.ThrowsException<SecurityException>(
				() => DeserializeHashtableValue("[]", "System.Collections.Generic.List`1[[AjaxPro.Tests.DeniedItem, AjaxPro.Tests]]"));
		}

		[TestMethod]
		public void AllowMode_DeniedTypeAsArrayElement_IsRejected()
		{
			AllowByDefaultDenyingDeniedItem();

			Assert.ThrowsException<SecurityException>(
				() => DeserializeHashtableValue("[]", "AjaxPro.Tests.DeniedItem[], AjaxPro.Tests"));
		}

		[TestMethod]
		public void AllowMode_TypeThatCannotBeFound_IsRejected()
		{
			AllowByDefaultDenyingDeniedItem();

			Assert.ThrowsException<SecurityException>(
				() => DeserializeHashtableValue("{}", "AjaxPro.Tests.DoesNotExist, AjaxPro.Tests"));
		}

		[TestMethod]
		public void AllowMode_MalformedTypeName_IsRejected()
		{
			AllowByDefaultDenyingDeniedItem();

			Assert.ThrowsException<SecurityException>(
				() => DeserializeHashtableValue("{}", "AjaxPro.Tests.Item[[, AjaxPro.Tests"));
		}
	}
}
