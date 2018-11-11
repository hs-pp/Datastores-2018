using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Datastores.Framework
{
    /// <summary>
    /// BaseDatastore is mostly an empty "base" class to be used to draw the Datastore Editor on.
    /// Because Unity's Custom Editors do not support drawing editors for generic classes (such as Datastore<T>),
    /// we must draw a custom editor for a parent class that all Datastores are inheriting from.
    /// </summary>
    public class BaseDatastore : ScriptableObject
    {
        
#if UNITY_EDITOR   
        
        /// <summary>
        /// Method to use when creating a new element. Implemented in Datastore<T> but could potentially be reimplemented
        /// if you need to run some logic on add.
        /// </summary>
        public virtual void AddNewElement()
        {
        }

        /// <summary>
        /// This method is needed in the EditorWindow(DatastoreWindow.cs) to draw the individual elements in the
        /// reorderable list. The actual implementation of DrawElement() should happen in any child class of
        /// DataElement.
        /// </summary>
        public virtual void DrawElement(Rect rect, SerializedProperty property)
        {
        }
#endif
        
    }
}