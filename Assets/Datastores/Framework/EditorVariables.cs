using System;
using UnityEngine;

namespace Datastores.Framework
{
    /// <summary>
    /// A serialized set of data only used in the DatastoreWindow.
    /// This is stored in the Datastore<T> scriptableObject to avoid having to save it out to disk in another way.
    /// Shooo you don't need to be here.
    /// </summary>
    [Serializable]
    public class EditorVariables
    {
        public float RelativeSizesX = 0.2f;
        public float RelativeSizesY = 0.8f;
        public Vector2 LeftScrollPos;
        public Vector2 RightScrollPos;
        public int ElementsPerPage = 15;
        public int CurrentPage = -1;
        public int TotalPages = -1;

        public EditorVariables()
        {
            LeftScrollPos = Vector2.zero;
            RightScrollPos = Vector2.zero;
        }
    }
}