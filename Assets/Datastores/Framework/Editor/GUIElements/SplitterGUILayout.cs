using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Datastores.Framework.Editor.GUIElements
{
	/// <summary>
	/// Reflectioned the SplitterGUILayout class out of Unity source to use for my own editor.
	/// bwahahahaha
	/// </summary>
	public static class SplitterGUILayout
	{
		private static readonly MethodInfo m_beginVerticalSplitMethod;
		private static readonly MethodInfo m_beginHorizontalSplitMethod;
		private static readonly MethodInfo m_endVerticalSplitMethod;
		private static readonly MethodInfo m_endHorizontalSplitMethod;

		private static Type SplitterGUILayoutType { get; }

		static SplitterGUILayout()
		{
			var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
			SplitterGUILayoutType = assembly.GetType("UnityEditor.SplitterGUILayout");
			m_beginVerticalSplitMethod = SplitterGUILayoutType.GetMethod("BeginVerticalSplit", new[] { SplitterState.SplitterStateType, typeof(GUILayoutOption[]) });
			m_beginHorizontalSplitMethod = SplitterGUILayoutType.GetMethod("BeginHorizontalSplit", new[] { SplitterState.SplitterStateType, typeof(GUILayoutOption[]) });
			m_endVerticalSplitMethod = SplitterGUILayoutType.GetMethod("EndVerticalSplit");
			m_endHorizontalSplitMethod = SplitterGUILayoutType.GetMethod("EndHorizontalSplit");
		}

		public static void BeginVerticalSplit(SplitterState state, params GUILayoutOption[] options)
		{
			m_beginVerticalSplitMethod.Invoke(null, new[] { state.InternalObject, options });
		}

		public static void BeginHorizontalSplit(SplitterState state, params GUILayoutOption[] options)
		{
			m_beginHorizontalSplitMethod.Invoke(null, new[] { state.InternalObject, options });
		}

		public static void EndVerticalSplit()
		{
			m_endVerticalSplitMethod.Invoke(null, null);
		}

		public static void EndHorizontalSplit()
		{
			m_endHorizontalSplitMethod.Invoke(null, null);
		}
	}
}