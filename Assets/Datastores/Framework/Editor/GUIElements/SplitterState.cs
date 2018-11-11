using System;
using System.Reflection;
using UnityEditor;

namespace Datastores.Framework.Editor.GUIElements
{
	/// <summary>
	/// Reflectioned the SplitterState class out of Unity source to use for my own editor.
	/// bwahahahaha
	/// </summary>
	public class SplitterState
	{
		public object InternalObject { get; }
		public static Type SplitterStateType { get; }

		static SplitterState()
		{
			var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
			SplitterStateType = assembly.GetType("UnityEditor.SplitterState");
		}

		public float[] relativeSizes
		{
			get
			{
				return SplitterStateType.GetField("relativeSizes").GetValue(InternalObject) as float[];
			}
			set
			{
				SplitterStateType.GetField("relativeSizes").SetValue(InternalObject, value);
			}
		}

		public SplitterState(float[] relativeSizes, int[] minSizes, int[] maxSizes)
		{
			InternalObject = Activator.CreateInstance(SplitterStateType, relativeSizes, minSizes, maxSizes);
		}
	}
}
