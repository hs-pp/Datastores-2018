using UnityEngine;
using UnityEditor;

namespace Datastores.Framework.Editor
{
    /// <summary>
    /// Custom Editor for all datastores.
    /// </summary>
    [CustomEditor(typeof(BaseDatastore), true)]
    public class BaseDatastoreEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Draws open button.
            if (GUILayout.Button("Open Datastore", GUILayout.Height(30)))
            {
                DatastoreWindow.Init(AssetDatabase.GetAssetPath(target));
            }

            GUILayout.Space(16);

            // Draw all other properties.
            EditorGUI.BeginChangeCheck();
            SerializedProperty prop = serializedObject.GetIterator();
            bool foundNext = prop.Next(true);
            while (foundNext)
            {
                if (prop.displayName != "Object Hide Flags" &&
                    prop.displayName != "Script" &&
                    prop.name != "m_elements" &&
                    prop.name != "m_editorVariables" &&
                    prop.name != "m_nextID")
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
                foundNext = prop.NextVisible(false);
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}