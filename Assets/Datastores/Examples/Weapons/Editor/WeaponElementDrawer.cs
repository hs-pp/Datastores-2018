using UnityEngine;
using UnityEditor;
using Datastores.Framework.Editor;

namespace Datastores.Examples
{
    /// <summary>
    /// This is the custom property drawer for the WeaponElement object. This is used when displaying the elements in
    /// the datastore editor. Without a custom property drawer, the datastore editor will use default property
    /// fields to draw out all the properties of each element.
    ///
    /// If you're not familiar with custom property drawers, please refer to Unity's documentation.
    /// </summary>
    [CustomPropertyDrawer(typeof(WeaponElement), true)]
    public class WeaponElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float origLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 85;
            GUILayout.BeginHorizontal();

            //Draw image
            SerializedProperty imageProp = property.FindPropertyRelative("m_icon");
            imageProp.objectReferenceValue = EditorGUILayout.ObjectField(imageProp.objectReferenceValue,
                typeof(Texture2D),
                false,
                GUILayout.Height(100),
                GUILayout.Width(100));

            GUILayout.BeginVertical();

            //Draw name and ID
            DataElementHelper.OnGUI_DrawDefaultValues(property);    
            /// This special method draws the element and it'd ID in a nicely formatted line.

            //Draw rarity and sell price
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_weaponType"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_rarity"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_sellPrice"));
            GUILayout.EndHorizontal();

            //Draw stats
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            StatDrawer(property.FindPropertyRelative("m_atk"));
            StatDrawer(property.FindPropertyRelative("m_def"));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            StatDrawer(property.FindPropertyRelative("m_spd"));
            StatDrawer(property.FindPropertyRelative("m_crit"));
            GUILayout.EndHorizontal();

            //Draw Prefab
            GUILayout.Space(2);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("m_prefab"));

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            EditorGUIUtility.labelWidth = origLabelWidth;
        }

        private void StatDrawer(SerializedProperty property)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label(property.displayName, GUILayout.Width(40));
            GUILayout.Space(4);

            Rect rect = EditorGUILayout.BeginVertical(GUILayout.Height(16), GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(rect, property.floatValue / 100f, "");
            GUILayout.Space(16);
            EditorGUILayout.EndVertical();

            GUILayout.Space(4);
            property.floatValue = EditorGUILayout.FloatField(property.floatValue, GUILayout.Width(40));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}