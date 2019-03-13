using UnityEngine;
using UnityEditor;

namespace Datastores.Framework.Editor
{
    /// <summary>
    /// Default PropertyDrawer for DataElements.
    /// This draws all the serialized properties raw without any pretty formatting.
    /// Write a custom property drawer for DataElement child classes to overrite this one.
    /// </summary>
    [CustomPropertyDrawer(typeof(DataElement), true)]
    public class DataElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Draw the DataElement elements.
            DataElementHelper.OnGUI_DrawDefaultValues(property);

            //Draw every other property raw.
            int depth = property.depth + 1;
            foreach (SerializedProperty childProp in property)
            {
                if (childProp.depth == depth && childProp.name != "m_id" && childProp.name != "m_name")
                {
                    EditorGUILayout.PropertyField(childProp, true);
                }
            }
        }
    }
}