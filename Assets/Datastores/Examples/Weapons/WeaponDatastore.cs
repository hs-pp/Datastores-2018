using UnityEngine;
using Datastores.Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Datastores.Examples
{
    /// <summary>
    /// This is the heart of your custom datastore.
    /// Every Datastore<T> has an internal List<T> to keep all of your elements.
    ///
    /// For any simple implementation of a datastore, there's really not much to do here.
    /// 
    /// But you also have the flexiblity to add any extra properties you may need. Any additional serializable
    /// properties will show up in this asset's inspector!
    /// </summary>
    /// 
    [CreateAssetMenu] /// <-- Make sure to include this attribute so you can actually create instances!!
    public class WeaponDatastore : Datastore<WeaponElement>
    {
		[SerializeField]
		public bool a;

		public string b;

		public Vector3 c;

#if UNITY_EDITOR
		//public override void DrawAssetInspector(SerializedObject so)
		//{
		//	GUILayout.Label("HGEYGFSDGDFGDFGDFGDFGDFGFGDG");
		//}
#endif
	}
}