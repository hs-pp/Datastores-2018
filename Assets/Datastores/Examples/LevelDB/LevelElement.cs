using UnityEngine;
using System.Collections.Generic;
using Datastores.Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelDatabase
{
	[System.Serializable]
	public class LevelElement : DataElement
	{
		[SerializeField]
		private SceneField m_scene;

		[SerializeField]
		private List<LevelComponent> m_components = new List<LevelComponent>();

		public T GetLevelComponent<T>() where T : LevelComponent
		{
			foreach (LevelComponent component in m_components)
			{
				if (component is T)
				{
					return component as T;
				}
			}
			return default(T);
		}


#if UNITY_EDITOR

		[SerializeField]
		private int m_componentTypeSelectIndex;

		public override void OnDrawElement(Rect rect, SerializedProperty property)
		{
			string scenePath = property.FindPropertyRelative("m_scene").FindPropertyRelative("m_scenePath").stringValue;
			bool sceneExists = scenePath != string.Empty;
			bool existsInBuildList = ExistsInBuildList(scenePath);

			GUILayout.BeginHorizontal();

			// Scene status
			Rect imageRect = rect;
			imageRect.width = 6;
			imageRect.height -= 2;
			DrawStatus(imageRect, sceneExists, existsInBuildList);

			// Name
			rect.width -= rect.height;
			rect.x += 8;
			EditorGUI.LabelField(rect, property.FindPropertyRelative("m_name").stringValue);

			GUILayout.EndHorizontal();
		}

		public static bool ExistsInBuildList(string scenePath)
		{
			foreach (EditorBuildSettingsScene buildSettingScene in EditorBuildSettings.scenes)
			{
				if (buildSettingScene.path == scenePath)
				{
					return true;
				}
			}
			return false;
		}

		public void DrawStatus(Rect rect, bool sceneExists, bool existsInBuildList)
		{
			Color origColor = GUI.color;
			if (!sceneExists)
			{
				GUI.color = Color.red;
			}
			else if (!existsInBuildList)
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.green;
			}
			GUI.Box(rect, "");
			GUI.color = origColor;
		}
#endif
	}
}
