using UnityEngine;

namespace LevelDatabase
{
	/// <summary>
	/// This is the default AttributesComponent.
	/// All custom components should be inherited from this class.
	/// Any variables in all derived classes should have the [SerializedField] tag in order for it to be seen in the LevelDB editor window.
	///
	///	Ex:
	///		[SerializeField]
	///		private int m_int1;
	///	
	/// Class functions will all work properly in this class.
	/// </summary>
	
	[System.Serializable]
	public class LevelComponent : ScriptableObject
	{
	}

}