using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Datastores.Framework
{
    /// <summary>
    /// The actual Datastore class. Inherit from this class to create a new type of Datastore.
    /// </summary>
    /// <typeparam name="T"> The class that models the data held in an element. </typeparam>
    public class Datastore<T> : BaseDatastore where T : DataElement
    {
        /// <summary>
        /// List of all the elements of type T.
        /// </summary>
        [SerializeField] protected List<T> m_elements = new List<T>();

        //============================================================================================================//
        // Universal Getter's of elements in the Datastore.
        // Any other getters can be implemented in a child class of Datastore<T> and still have access to these Getters.
        //============================================================================================================//
        /// <summary>
        /// Get an element by provided id.
        /// If none is found, returns null.
        /// </summary>
        public T GetElementByID(string id)
        {
            return m_elements.Find(x => x.Id.Equals(id));
        }

        /// <summary>
        /// Get an element by provided name.
        /// If none is found, returns null.
        /// </summary>
        public T GetElementByName(string name)
        {
            return m_elements.Find(x => x.Name == name);
        }

        /// <summary>
        /// Returns all elements in the datastore.
        /// </summary>
        public IEnumerable<T> GetAllElements()
        {
            return m_elements.AsReadOnly();
        }

        //============================================================================================================//
        // Properties only used from the editor. Don't worry about these.
        //============================================================================================================//
#if UNITY_EDITOR
        
        /// <summary>
        /// A serialized set of data only used in the DatastoreWindow.
        /// These are intentionally saved here so that closing and reopening the window does not reset any visual
        /// properties.
        /// Because these are wrapped in the UNITY_EDITOR symbol, the object won't be stored in a build.
        /// </summary>
        [SerializeField] private EditorVariables m_editorVariables;

		/// <summary>
		/// On add new element, we must give the newly created element a unique id before adding it to our list of elements.
		/// </summary>
        public override void AddNewElement()
        {
            T newElement = (T)Activator.CreateInstance(typeof(T));
            newElement.Id = GUID.Generate().ToString();
            m_elements.Add(newElement);
        }

        /// <summary>
        /// Flowthrough to call the specific T's OnDrawElement() function.
        /// If one does not exist, it will default back to DataElement's impl of OnDrawElement().
        /// </summary>
        public override void DrawElement(Rect rect, SerializedProperty property)
        {
            if(m_elements.Count > 0)
                m_elements[0].OnDrawElement(rect, property);
        }
#endif
    }
}
