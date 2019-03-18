using UnityEngine;
using UnityEditor;
using LevelDatabase.Framework;

namespace LevelDatabase
{
	[CustomPropertyDrawer(typeof(SceneField))]
	public class SceneFieldPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
		{
			EditorGUI.BeginProperty(_position, GUIContent.none, _property);
			SerializedProperty sceneAsset = _property.FindPropertyRelative("m_sceneAsset");
			//SerializedProperty sceneName = _property.FindPropertyRelative("m_scenePath");
			_position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
			if (sceneAsset != null)
			{
				EditorGUI.BeginChangeCheck();
				sceneAsset.objectReferenceValue = EditorGUI.ObjectField(_position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
				if(EditorGUI.EndChangeCheck())
				{
					if(sceneAsset.objectReferenceValue == null)
					{
						_property.FindPropertyRelative("m_scenePath").stringValue = string.Empty;
					}
				}
				//EditorGUILayout.LabelField("Name: " + sceneName.stringValue);
			}
			EditorGUI.EndProperty();
		}
	}
}