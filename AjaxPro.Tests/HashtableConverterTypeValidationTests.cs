using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AjaxPro.Tests
{
	/// <summary>
	/// The Hashtable wire format carries the key and value type names per entry:
	/// [[key, value, keyTypeName, valueTypeName], ...]. These names come from the
	/// request, so they have to pass the jsonDeserializationCustomTypes check
	/// before Type.GetType() gets to load the assembly they name.
	///
	/// The tests run without an ajaxNet/ajaxSettings section, so the defaults
	/// apply: custom types are denied and nothing is on the allow list.
	/// </summary>
	[TestClass]
	public class HashtableConverterTypeValidationTests
	{
		private const string MissingAssembly = "AjaxProTestsAssemblyThatDoesNotExist";
		private const string UnlistedTypeName = "Some.Unlisted.Type, " + MissingAssembly;

		private List<string> m_ResolveRequests;

		[TestInitialize]
		public void Initialize()
		{
			m_ResolveRequests = new List<string>();
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}

		[TestCleanup]
		public void Cleanup()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
		}

		// Type.GetType() raises AssemblyResolve when it cannot find the assembly
		// of an assembly-qualified name, which tells us it was asked to load it.
		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith(MissingAssembly, StringComparison.Ordinal))
				m_ResolveRequests.Add(args.Name);

			return null;
		}

		[TestMethod]
		public void UnlistedValueTypeName_IsRejectedBeforeTheTypeIsLoaded()
		{
			string json = "[[\"key\",\"value\",\"System.String\",\"" + UnlistedTypeName + "\"]]";

			Assert.ThrowsException<SecurityException>(
				() => JavaScriptDeserializer.DeserializeFromJson(json, typeof(Hashtable)));

			Assert.AreEqual(0, m_ResolveRequests.Count, "Type.GetType() was called with the unvalidated type name.");
		}

		[TestMethod]
		public void UnlistedKeyTypeName_IsRejectedBeforeTheTypeIsLoaded()
		{
			string json = "[[\"key\",\"value\",\"" + UnlistedTypeName + "\",\"System.String\"]]";

			Assert.ThrowsException<SecurityException>(
				() => JavaScriptDeserializer.DeserializeFromJson(json, typeof(Hashtable)));

			Assert.AreEqual(0, m_ResolveRequests.Count, "Type.GetType() was called with the unvalidated type name.");
		}

		[TestMethod]
		public void BasicTypeNames_AreStillDeserialized()
		{
			string json = "[[\"key\",\"value\",\"System.String\",\"System.String\"]]";

			Hashtable result = (Hashtable)JavaScriptDeserializer.DeserializeFromJson(json, typeof(Hashtable));

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("value", result["key"]);
		}
	}
}
