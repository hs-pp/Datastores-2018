using UnityEngine;
using Datastores.Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Datastores.Examples
{
	/// <summary>
	/// This is your element model. Add all your useful properties here.
	///
	/// Make sure this class as well as your properties are all serialized if you want them to be editable in the
	/// datastore editor.
	/// 
	/// The datastore system supports custom property drawers so you also have the ability to it look nice in the
	/// datastore editor! (Refer to WeaponElementDrawer.cs for an example.)
	/// </summary>
	[System.Serializable] ///<-- This attribute is REQUIRED for Unity to serialize this class.
	public class WeaponElement : DataElement
	{
		[SerializeField] private Texture2D m_icon;
		[SerializeField] private GameObject m_prefab;
		[SerializeField] private float m_atk;
		[SerializeField] private float m_def;
		[SerializeField] private float m_spd;
		[SerializeField] private float m_crit;
		[SerializeField] private WeaponType m_weaponType;
		[SerializeField] private WeaponRarity m_rarity;
		[SerializeField] private int m_sellPrice;
		[SerializeField] private AnimationCurve m_crazyCurve;

		
#if UNITY_EDITOR
		/// <summary>
		/// This is a very special, overridable method that draws the element in the ReorderableList View in the
		/// datastore editor. If needed, make sure to wrap this method in an #if UNITY_EDITOR.
		/// </summary>
		public override void OnDrawElement(Rect rect, SerializedProperty property)
		{
			GUILayout.BeginHorizontal();

			// Draw icon
			Rect imageRect = rect;
			imageRect.width = imageRect.height;
			object icon = property.FindPropertyRelative("m_icon").objectReferenceValue;
			if (icon != null)
				GUI.DrawTexture(imageRect, icon as Texture2D);

			// Draw name
			rect.width -= rect.height;
			rect.x += rect.height;
			EditorGUI.LabelField(rect, property.FindPropertyRelative("m_name").stringValue);

			GUILayout.EndHorizontal();
		}
#endif
	}
}