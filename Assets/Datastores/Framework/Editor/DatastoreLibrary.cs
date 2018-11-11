using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Datastores.Framework.Editor
{
    /// <summary>
    /// EditorWindow to display a list of all the Datastore assets in the project.
    /// </summary>
    public class DatastoreLibrary : EditorWindow
    {
        private List<string> m_typesByName = new List<string>();
        private List<List<BaseDatastore>> m_datastores = new List<List<BaseDatastore>>();
        
        private Vector2 scrollPos;

        [MenuItem("Window/Datastore Library")]
        public static void Init()
        {
            DatastoreLibrary instance = GetWindow<DatastoreLibrary>("Library");
            instance.Show(true);
            instance.Focus();
        }

        private void OnEnable()
        {
            GetSortedDatastoreTypes();
            GetAllExistingDatastore();
        }

        /// <summary>
        /// Populates m_typesByName.
        /// </summary>
        private void GetSortedDatastoreTypes()
        {
            m_typesByName.Clear();
            List<MonoScript> allScripts = FindScripts.FindAllScripts(typeof(BaseDatastore));
            for (int i = 0; i < allScripts.Count; i++)
            {
                m_typesByName.Add(allScripts[i].GetClass().Name);
            }

            m_typesByName.Sort();
        }
        
        /// <summary>
        /// Populates m_datastores.
        /// </summary>
        private void GetAllExistingDatastore()
        {
            m_datastores.Clear();
            for (int i = 0; i < m_typesByName.Count; i++)
            {
                m_datastores.Add(new List<BaseDatastore>());
            }

            string[] assets = AssetDatabase.FindAssets("t:BaseDatastore", null);
            for (int i = 0; i < assets.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[i]);
                BaseDatastore datastore = AssetDatabase.LoadAssetAtPath<BaseDatastore>(path);
                int index = m_typesByName.FindIndex(x => x == datastore.GetType().Name.ToString());
                m_datastores[index].Add(datastore);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            //========================================================================================================//
            // Title
            //========================================================================================================//
            GUILayout.BeginHorizontal(EditorConstants.BoxStyle);
            DrawCenteredLabel("Datastore Library", EditorConstants.DatastoreTitleStyle);
            GUILayout.EndHorizontal();

            //========================================================================================================//
            // Draw Existing Datastores
            //========================================================================================================//
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginVertical(EditorConstants.BoxStyle);
            DrawCenteredLabel("Existing Datastores");
            GUILayout.Space(10);
            for (int i = 0; i < m_typesByName.Count; i++)
            {
                DrawCenteredLabel(m_typesByName[i], EditorConstants.LibraryTypeStyle);
                
                List<BaseDatastore> storesOfSameType = m_datastores[i];
                for (int j = 0; j < storesOfSameType.Count; j++)
                {
                    GUILayout.Space(2);
                    
                    GUILayout.Label((j + 1) + ": " + storesOfSameType[j].name, 
                        EditorConstants.LibraryDatastoreNameStyle);
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(storesOfSameType[j], typeof(BaseDatastore), false);
                    GUI.enabled = true;

                    if (GUILayout.Button("Open Datastore", GUILayout.Height(30)))
                    {
                        DatastoreWindow.Init(AssetDatabase.GetAssetPath(storesOfSameType[j]));
                    }

                    GUILayout.Space(4);
                }

                GUILayout.Space(16);
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            //========================================================================================================//
            // Draw the refresh button.
            //========================================================================================================//
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh", GUILayout.Width(64)))
            {
                GetSortedDatastoreTypes();
                GetAllExistingDatastore();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Generic centered label.
        /// </summary>
        private void DrawCenteredLabel(string label, GUIStyle style = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, style ?? GUIStyle.none);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
