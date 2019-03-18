using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace LevelDatabase
{
	public static class FindScripts
	{
		/// <summary>
		/// Finds all scripts based on a function.
		/// </summary>
		/// <returns>All the scripts where the specified function returned true.</returns>
		/// <param name="p">Function to determine which scripts to include.</param>
		public static List<MonoScript> FindAllScripts(Func<MonoScript, bool> p)
		{
			List<MonoScript> scriptsReturn = new List<MonoScript>();

			MonoScript[] allScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
			foreach (MonoScript scr in allScripts)
			{
				if (p.Invoke(scr))
				{
					scriptsReturn.Add(scr);
				}
			}

			return scriptsReturn;
		}

		private static Dictionary<Type, List<MonoScript>> m_typeScriptMap = new Dictionary<Type, List<MonoScript>>(100);

		/// <summary>
		/// Finds all scripts that contain classes that derive from a specified type.
		/// This includes derived from classes or implemented interfaces.
		/// </summary>
		/// <returns>All of the scripts of the type specified.</returns>
		/// <param name="typeToFind">Type to find.</param>
		public static List<MonoScript> FindAllScripts(Type typeToFind)
		{
			List<MonoScript> scriptsReturn;
			if (!m_typeScriptMap.TryGetValue(typeToFind, out scriptsReturn))
			{
				scriptsReturn = new List<MonoScript>(100);

				m_typeScriptMap[typeToFind] = scriptsReturn;

				string typeName = typeToFind.Name;
				MonoScript[] allScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
				foreach (MonoScript scr in allScripts)
				{
					Type clsType = scr.GetClass();
					if (clsType != null && (clsType.IsSubclassOf(typeToFind) || clsType.GetInterface(typeName) != null))
					{
						scriptsReturn.Add(scr);
					}
				}
			}

			return new List<MonoScript>(scriptsReturn);
		}

		/// <summary>
		/// Finds all scripts with a specified attribute.
		/// </summary>
		/// <returns>All scripts with the specified attribute type.</returns>
		/// <param name="attributeType">Attribute type to look for in scripts.</param>
		public static List<MonoScript> FindAllScriptsWithAttribute(Type attributeType)
		{
			List<MonoScript> scriptsReturn;
			if (!m_typeScriptMap.TryGetValue(attributeType, out scriptsReturn))
			{
				scriptsReturn = new List<MonoScript>(100);
				m_typeScriptMap[attributeType] = scriptsReturn;

				MonoScript[] allScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
				foreach (MonoScript scr in allScripts)
				{
					Type clsType = scr.GetClass();
					if (clsType != null)
					{
						object[] attrs = clsType.GetCustomAttributes(attributeType, true);
						if (attrs.Length > 0)
						{
							scriptsReturn.Add(scr);
						}
					}
				}
			}

			return scriptsReturn;
		}

		public static List<object> FindAllAttributesOfType<T>()
		{
			List<object> attrList = new List<object>();
			MonoScript[] allScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
			foreach (MonoScript scr in allScripts)
			{
				System.Type clsType = scr.GetClass();
				if (clsType != null)
				{
					object[] attrs = clsType.GetCustomAttributes(typeof(T), true);
					attrList.AddRange(attrs);
				}
			}
			return attrList;
		}
	}
}