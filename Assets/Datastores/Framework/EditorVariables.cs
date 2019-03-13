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
        public float RelativeLeftPanelRatio = 0.2f;
		public bool ShowAssetInspector = false;
		public float RelativeAssetInspectorSize = 0.2f;
        public Vector2 LeftScrollPos;
        public Vector2 RightScrollPos;
		public Vector2 AssetInspectorScrollPos;
        public int ElementsPerPage = 15;
        public int CurrentPage = -1;
        public int TotalPages = -1;
    }
}