using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.Collections.Generic;

namespace LevelDatabase
{
	[CustomPropertyDrawer(typeof(LevelElement), true)]
	public class LevelElementPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty sceneProp = property.FindPropertyRelative("m_scene");
			string scenePath = sceneProp.FindPropertyRelative("m_scenePath").stringValue;
			bool sceneExists = scenePath != string.Empty;
			bool existsInBuildList = sceneExists ? LevelElement.ExistsInBuildList(scenePath) : false;

			GUILayout.Space(6);
			GUILayout.BeginHorizontal();

			DrawStatus(sceneExists, existsInBuildList);
		
			GUILayout.BeginVertical();
			Datastores.Framework.Editor.DataElementHelper.OnGUI_DrawDefaultValues(property);
			GUILayout.Space(4);
			EditorGUILayout.PropertyField(sceneProp);
			GUILayout.Space(6);
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			DrawComponents(property.FindPropertyRelative("m_components"));
			GUILayout.Space(4);
			DrawNewComponentButton(property);

			GUILayout.Space(10);
		}

		private void DrawStatus(bool sceneExists, bool existsInBuildList)
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
			GUILayout.Box("", GUILayout.Width(6), GUILayout.Height(40));
			GUI.color = origColor;
		}

		private void DrawComponents(SerializedProperty componentsProp)
		{
			GUILayout.Space(16);
			for(int i = 0; i < componentsProp.arraySize; i++)
			{
				GUILayout.BeginHorizontal();

				EditorGUILayout.BeginVertical(new GUIStyle("Box"), GUILayout.ExpandWidth(true));

				Object elementRef = componentsProp.GetArrayElementAtIndex(i).objectReferenceValue;
				SerializedObject component = null;
				if (elementRef)
				{
					component = new SerializedObject(elementRef);
				}


				GUILayout.BeginHorizontal(EditorStyles.toolbar);
				if (i > 0 && componentsProp.arraySize != 1)
				{
					if (GUILayout.Button("/\\", EditorStyles.toolbarButton))
					{
						ReorderComponents(componentsProp, i, -1);
					}
				}
				else
				{
					GUILayout.Button("  ", EditorStyles.toolbarButton);
				}
				if (i < componentsProp.arraySize - 1)
				{
					if (GUILayout.Button("\\/", EditorStyles.toolbarButton))
					{
						ReorderComponents(componentsProp, i, 1);
					}
				}
				else
				{
					GUILayout.Button("  ", EditorStyles.toolbarButton);
				}
				GUILayout.FlexibleSpace();
				GUILayout.Label(elementRef != null? elementRef.GetType().Name: "");
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("X", EditorStyles.toolbarButton))
				{
					DeleteLevelComponent(componentsProp, i);
					return;
				}
				GUILayout.EndHorizontal();

				if(component != null)
				{
					SerializedProperty prop = component.GetIterator();
					prop.NextVisible(true);
					do
					{
						if (prop.displayName == "Script") { GUI.enabled = false; }
						EditorGUILayout.PropertyField(prop, true);
						if (prop.displayName == "Script") { GUI.enabled = true; }
					} while (prop.NextVisible(false));
					component.ApplyModifiedProperties();
				}
				else
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label("<color=red>Component script is missing!</color>", new GUIStyle { richText = true });
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(4);
				EditorGUILayout.EndVertical();
				
				GUILayout.EndHorizontal();
				GUILayout.Space(6);
			}
		}
		private void DrawNewComponentButton(SerializedProperty levelElementProperty)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			List<MonoScript> allScripts = FindScripts.FindAllScripts(typeof(LevelComponent));
			System.Type[] types = new System.Type[allScripts.Count + 1];
			string[] typesByName = new string[allScripts.Count + 1];
			typesByName[0] = "---";
			for (int i = 0; i < allScripts.Count; i++)
			{
				types[i + 1] = allScripts[i].GetClass();
				typesByName[i + 1] = types[i + 1].Name;
			}
			SerializedProperty selectIndex = levelElementProperty.FindPropertyRelative("m_componentTypeSelectIndex");
			selectIndex.intValue = EditorGUILayout.Popup(selectIndex.intValue, typesByName);
			if (GUILayout.Button("Add Level Component"))
			{
				if (selectIndex.intValue != 0)
				{
					AddLevelComponent(levelElementProperty, types[selectIndex.intValue]);
					selectIndex.intValue = 0;
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void AddLevelComponent(SerializedProperty levelElementProperty, System.Type classToCreate)
		{
			//check if already exists.
			SerializedProperty componentList = levelElementProperty.FindPropertyRelative("m_components");
			for(int i = 0; i < componentList.arraySize; i++)
			{
				System.Type typeOfComponent = componentList.GetArrayElementAtIndex(i).objectReferenceValue.GetType();
				if(typeOfComponent == classToCreate)
				{
					if(EditorUtility.DisplayDialog("Error!", "This component already exists!", "Okay"))
					{
						return;
					}
				}
			}

			//add component
			LevelComponent newComponent = ScriptableObject.CreateInstance(classToCreate) as LevelComponent;
			newComponent.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(newComponent, levelElementProperty.serializedObject.targetObject);

			componentList.InsertArrayElementAtIndex(componentList.arraySize);
			componentList.GetArrayElementAtIndex(componentList.arraySize - 1).objectReferenceValue = newComponent;
		}

		private void DeleteLevelComponent(SerializedProperty componentsProp, int index)
		{
			if (!EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this component?", "Yes", "No"))
			{
				return;
			}
			ScriptableObject obj = componentsProp.GetArrayElementAtIndex(index).objectReferenceValue as ScriptableObject;
			componentsProp.GetArrayElementAtIndex(index).objectReferenceValue = null;
			componentsProp.DeleteArrayElementAtIndex(index);
			UnityEngine.Object.DestroyImmediate(obj, true);
			componentsProp.serializedObject.ApplyModifiedProperties();
		}

		private void ReorderComponents(SerializedProperty componentsProp, int index, int direction)
		{
			int sceneDataSize = componentsProp.arraySize;
			componentsProp.MoveArrayElement(index, index + (1 * direction));
			componentsProp.serializedObject.ApplyModifiedProperties();
		}

		private void CheckIfSceneExists(SerializedProperty sceneObject)
		{
			if (sceneObject.objectReferenceValue == null)
			{
				return;
			}
		}

	}
}