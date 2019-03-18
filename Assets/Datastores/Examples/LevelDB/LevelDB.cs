using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Datastores.Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelDatabase
{
	[CreateAssetMenu]
	public class LevelDB : Datastore<LevelElement>
	{

#if UNITY_EDITOR
		public override void DrawAssetInspector(SerializedObject so)
		{
			DefaultDrawAssetInspector(so);
		}
#endif
	}
}
