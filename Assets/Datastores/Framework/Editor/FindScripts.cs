using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Datastores.Framework.Editor
{
	public static class FindScripts
	{
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
	}
}