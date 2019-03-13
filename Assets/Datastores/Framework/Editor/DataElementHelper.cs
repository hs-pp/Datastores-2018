using UnityEngine;
using UnityEditor;

namespace Datastores.Framework.Editor
{
    public static class DataElementHelper
    {
        /// <summary>
        /// Draws the default values of a DataElement.
        /// When implementing your own property drawer, make sure to call this to display the ID and name of the element 
        /// </summary>
        public static void OnGUI_DrawDefaultValues(SerializedProperty property)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label(" Name: ", EditorConstants.DataElementNameStyle, GUILayout.Width(48));
            
            property.FindPropertyRelative("m_name").stringValue =
                GUILayout.TextField(property.FindPropertyRelative("m_name").stringValue, GUILayout.Width(250));

            GUILayout.Label(string.Format("ID: {0}", property.FindPropertyRelative("m_id").stringValue), EditorConstants.DataElementIDStyle);
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }
    }
}