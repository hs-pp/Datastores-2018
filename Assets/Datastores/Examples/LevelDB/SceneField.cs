using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelDatabase
{
	[System.Serializable]
	public class SceneField : ISerializationCallbackReceiver
	{
		[SerializeField]
		private Object m_sceneAsset;
		[SerializeField]
		private string m_scenePath = "";
		public string ScenePath
		{
			get { return m_scenePath; }
		}
		// makes it work with the existing Unity methods (LoadLevel/LoadScene)
		public static implicit operator string(SceneField sceneField)
		{
			return sceneField.ScenePath;
		}


		private bool IsValidSceneAsset()
		{
			if (m_sceneAsset == null)
				return false;
			return m_sceneAsset.GetType().Equals(typeof(SceneAsset));
		}

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			HandleBeforeSerialize();
#endif
		}
		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			EditorApplication.update += HandleAfterDeserialize;
#endif
		}

#if UNITY_EDITOR
		private SceneAsset GetSceneAssetFromPath()
		{
			if (string.IsNullOrEmpty(m_scenePath))
				return null;
			return AssetDatabase.LoadAssetAtPath<SceneAsset>(m_scenePath);
		}

		private string GetScenePathFromAsset()
		{
			if (m_sceneAsset == null)
				return string.Empty;
			return AssetDatabase.GetAssetPath(m_sceneAsset);
		}

		private void HandleBeforeSerialize()
		{
			if (!IsValidSceneAsset() && string.IsNullOrEmpty(m_scenePath) == false)
			{
				m_sceneAsset = GetSceneAssetFromPath();
				if (m_sceneAsset == null)
					m_scenePath = string.Empty;

				UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
			}
			else
			{
				m_scenePath = GetScenePathFromAsset();
			}
		}

		private void HandleAfterDeserialize()
		{
			EditorApplication.update -= HandleAfterDeserialize;

			if (IsValidSceneAsset())
				return;
			
			if (string.IsNullOrEmpty(m_scenePath) == false)
			{
				m_sceneAsset = GetSceneAssetFromPath();
				if (m_sceneAsset == null)
					m_scenePath = string.Empty;

				if (Application.isPlaying == false)
					UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
			}
		}
#endif
	}
}