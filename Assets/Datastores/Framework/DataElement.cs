using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Datastores.Framework
{
    /// <summary>
    /// The base DataElement to be inherited from.
    /// </summary>
    [System.Serializable]
    public class DataElement
    {
        //============================================================================================================//
        // All DataElements need an ID and a Name to be identified by.
        // These will exist in any other inherited class.
        //============================================================================================================//
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        
        public string Id
        {
            get { return m_id; }

#if UNITY_EDITOR
            set { m_id = value; }
#endif
        }

        public string Name
        {
            get { return m_name; }
        }
        
        //============================================================================================================//
        // Properties only used from the editor. 
        //============================================================================================================//
#if UNITY_EDITOR
        /// <summary>
        /// Implement this method if you want to modify how the element is displayed in the editor window's
        /// reorderable list.
        /// </summary>
        public virtual void OnDrawElement(Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, property.FindPropertyRelative("m_name").stringValue);
        }
#endif
    }
}