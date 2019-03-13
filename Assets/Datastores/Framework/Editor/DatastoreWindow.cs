using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Datastores.Framework.Editor.GUIElements;
using UnityEditor.Callbacks;

namespace Datastores.Framework.Editor
{
	/// <summary>
	/// The Datastore window - itself. D:
	/// </summary>
	public class DatastoreWindow : EditorWindow
	{
		//============================================================================================================//
		// General Editor Variables
		//============================================================================================================//
		public string DataStorePATH { get; set; }
		private BaseDatastore m_dataStore;
		private SerializedObject m_so;
		private SerializedProperty m_elementsList;

		//============================================================================================================//
		// GUI Variables
		//============================================================================================================//
		private EditorVariables m_editorVars;
		private SplitterState m_splitterState;
		private bool m_showAssetInspector;
		private SplitterState m_assetInspectorSplitterState;
		private ReorderableList m_reorderableList;
		private float[] m_listViewRectYPositions;
		private int m_elementToFocus = -1;

		//============================================================================================================//
		// Names of the properties in the datastore serialized object.
		//============================================================================================================//
		private const string ELEMENTS_PATH = "m_elements";
		private const string EDITOR_VARIABLES_PATH = "m_editorVariables";
		private const string NEXTID_PATH = "m_nextID";

		private const string RELATIVE_LEFT_PANEL_RATIO = "RelativeLeftPanelRatio";
		private const string SHOW_ASSET_INSPECTOR = "ShowAssetInspector";
		private const string RELATIVE_ASSET_INSPECTOR_SIZE = "RelativeAssetInspectorSize";
		private const string LEFT_SCROLL_POS = "LeftScrollPos";
		private const string RIGHT_SCROLL_POS = "RightScrollPos";
		private const string ASSET_INSPECTOR_SCROLL_POS = "AssetInspectorScrollPos";
		private const string ELEMENTS_PER_PAGE = "ElementsPerPage";
		private const string CURRENT_PAGE = "CurrentPage";
		private const string TOTAL_PAGES = "TotalPages";

		#region Init
		//============================================================================================================//
		// Init
		//============================================================================================================//
		/// <summary>
		/// Called to create a new Datastore window.
		/// </summary>
		public static void Init(string dataStorePath)
		{
			DatastoreWindow instance = GetWindow<DatastoreWindow>("Datastore");
			instance.DataStorePATH = dataStorePath;
			instance.SetDatabase();
			instance.Show(true);
			instance.Focus();
		}

		/// <summary>
		/// In the constructor,
		/// - Create instance of SplitterState
		/// - Subscribe to events related to editor recompilation
		/// </summary>
		private DatastoreWindow()
		{
			m_splitterState = new SplitterState(new[] { 0.2f, 0.8f }, new[] { 170, 170 }, null);
			m_assetInspectorSplitterState = new SplitterState(new[] { 0.8f, 0.2f }, new[] { 100, 100 }, null);

			//========================================================================================================//
			// Set up on-compile callbacks
			//========================================================================================================//
			//Reload the datastore in the window
			AssemblyReloadEvents.afterAssemblyReload -= SetDatabase;
			AssemblyReloadEvents.afterAssemblyReload += SetDatabase;

			//Save editor vars before compiling
			AssemblyReloadEvents.beforeAssemblyReload -= SerializeEditorVars;
			AssemblyReloadEvents.beforeAssemblyReload += SerializeEditorVars;
		}

		/// <summary>
		/// Save editor variables before closing window.
		/// </summary>
		private void OnDisable()
		{
			//Save editor vars whenever window closes.
			SerializeEditorVars();
		}

		/// <summary>
		/// Loads the datastore and editor variables.
		/// </summary>
		private void SetDatabase()
		{
			//Find the datastore asset using the saved PATH
			m_dataStore = AssetDatabase.LoadAssetAtPath<BaseDatastore>(DataStorePATH);
			m_so = new SerializedObject(m_dataStore);

			//Setup local vars
			m_elementsList = m_so.FindProperty(ELEMENTS_PATH);
			if (m_elementsList == null)
			{
				return;
			}
			m_reorderableList = new ReorderableList(m_so, m_elementsList);

			//Load editor vars
			DeserializeEditorVars();
		}
		#endregion

		#region OnGUI
		//============================================================================================================//
		// OnGUI
		//============================================================================================================//
		public void OnGUI()
		{
			if (!ErrorTestsPass()) return;

			//Get any updates since the last frame.
			m_so.Update();

			CheckForEditorVarsChanges();

			DrawToolbar();
			GUILayout.BeginVertical();
			DrawTitle();

			SplitterGUILayout.BeginHorizontalSplit(m_splitterState);
			if(m_showAssetInspector)
			{
				DrawLeftPanel_WithAssetInspector();
			}
			else
			{
				DrawLeftPanel();
			}
			DrawRightPanel();
			SplitterGUILayout.EndHorizontalSplit();

			GUILayout.EndVertical();

			//Save all changes made this frame.
			m_so.ApplyModifiedProperties();
		}

		/// <summary>
		/// Draws the toolbar above the window.
		/// </summary>
		private void DrawToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			if (GUILayout.Button(" Datastore Library ", EditorStyles.toolbarButton))
			{
				DatastoreLibrary.Init();
			}

			GUILayout.FlexibleSpace();
			if (m_so != null)
			{
				GUILayout.Label("ElementsPerPage");
				SerializedProperty editorVars = m_so.FindProperty(EDITOR_VARIABLES_PATH);
				SerializedProperty elementsPerPage = editorVars.FindPropertyRelative("ElementsPerPage");

				EditorGUI.BeginChangeCheck();
				elementsPerPage.intValue = Mathf.Clamp(EditorGUILayout.IntField(elementsPerPage.intValue,
					GUILayout.Height(14), GUILayout.Width(22)), 1, 99);
				if (EditorGUI.EndChangeCheck())
				{
					editorVars.FindPropertyRelative("CurrentPage").intValue = -1;
					editorVars.FindPropertyRelative("TotalPages").intValue = -1;
					m_so.ApplyModifiedProperties();
				}

				GUILayout.Space(6);

				m_showAssetInspector = GUILayout.Toggle(m_showAssetInspector, "Show Asset Inspector", EditorStyles.toolbarButton);
			}
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// Draws the name of the Datastore as the title of the window.
		/// </summary>
		private void DrawTitle()
		{
			GUILayout.BeginVertical(EditorConstants.BoxStyle, GUILayout.ExpandWidth(true));
			GUILayout.Space(4);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(m_dataStore.name, EditorConstants.DatastoreTitleStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(4);
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws everything on the left panel of the splitterState.
		/// </summary>
		private void DrawLeftPanel()
		{
			GUILayout.BeginVertical(EditorConstants.BoxStyle, GUILayout.ExpandHeight(true));
			m_editorVars.LeftScrollPos = GUILayout.BeginScrollView(m_editorVars.LeftScrollPos);
			DrawReorderable();
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}

		private void DrawLeftPanel_WithAssetInspector()
		{
			GUILayout.BeginVertical();
			SplitterGUILayout.BeginVerticalSplit(m_assetInspectorSplitterState);

			DrawLeftPanel();
			DrawAssetInspector();

			SplitterGUILayout.EndHorizontalSplit();
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws everything on the right panel of the splitterState.
		/// </summary>
		private void DrawRightPanel()
		{
			// Adding space between left and right panels.
			GUILayout.BeginHorizontal();
			GUILayout.Space(4);

			// Actual panel
			GUILayout.BeginVertical(EditorConstants.BoxStyle, GUILayout.ExpandHeight(true));

			DrawTopControls();

			m_editorVars.RightScrollPos = GUILayout.BeginScrollView(m_editorVars.RightScrollPos);
			DrawListView();
			GUILayout.EndScrollView();

			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
		}

		private void DrawAssetInspector()
		{
			GUILayout.BeginVertical(EditorConstants.BoxStyle, GUILayout.ExpandWidth(true));
			m_editorVars.AssetInspectorScrollPos = GUILayout.BeginScrollView(m_editorVars.AssetInspectorScrollPos);

			GUILayout.Space(4);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Asset Inspector");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(6);

			m_dataStore.DrawAssetInspector(m_so);

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draw the reorderablelist using the serialized object.
		/// </summary>
		private void DrawReorderable()
		{
			GUILayout.BeginVertical();
			m_reorderableList.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, string.Format("-{0} Elements-", m_reorderableList.count));
			};
			m_reorderableList.drawElementCallback = (Rect rect, int i, bool isActive, bool isFocused) =>
			{
				int spaceForIndicator = 16;

				Rect elementRect = rect;
				elementRect.width -= spaceForIndicator;
				m_dataStore.DrawElement(elementRect, m_elementsList.GetArrayElementAtIndex(i));

				if (i / m_editorVars.ElementsPerPage == m_editorVars.CurrentPage - 1)
				{
					Rect asteriskRect = rect;
					asteriskRect.width = spaceForIndicator;
					asteriskRect.x = rect.x + rect.width - spaceForIndicator;
					GUI.Label(asteriskRect, "^");
				}
			};
			m_reorderableList.onAddCallback = (ReorderableList reorderableList) =>
			{
				m_dataStore.AddNewElement();
				DeterminePages();
			};
			m_reorderableList.onRemoveCallback = (ReorderableList reorderableList) =>
			{
				if (EditorUtility.DisplayDialog("Warning!",
					"Are you sure you want to delete this element?", "Yes", "No"))
				{
					ReorderableList.defaultBehaviours.DoRemoveButton(reorderableList);
					DeterminePages();
				}
			};
			m_reorderableList.onSelectCallback = (ReorderableList reorderableList) =>
			{
				OnElementSelect(reorderableList.index);
			};
			m_reorderableList.DoLayoutList();
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws the list of elements using their property drawers.
		/// </summary>
		private void DrawListView()
		{
			int startIndex = (m_editorVars.CurrentPage - 1) * m_editorVars.ElementsPerPage;
			int remaining = m_elementsList.arraySize - startIndex;
			int endIndex = startIndex + (remaining < m_editorVars.ElementsPerPage ? remaining : m_editorVars.ElementsPerPage);
			m_listViewRectYPositions = new float[endIndex - startIndex];
			for (int i = startIndex; i < endIndex; i++)
			{
				m_listViewRectYPositions[i - startIndex] = DrawElement(m_elementsList.GetArrayElementAtIndex(i));
			}

			TryFocusElement();
		}

		/// <summary>
		/// Draws a single element using its property drawer.
		/// </summary>
		private float DrawElement(SerializedProperty prop)
		{
			Rect rect = EditorGUILayout.BeginVertical(EditorConstants.BoxStyle);
			EditorGUI.PropertyField(rect, prop);
			EditorGUILayout.EndVertical();
			return rect.y;
		}

		/// <summary>
		/// Draws all the controls on top of the ListView.
		/// Currently this is only the pagination controls.
		/// </summary>
		private void DrawTopControls()
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			DrawPagination();
			GUILayout.EndHorizontal();
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws the elements for the pagination controls.
		/// </summary>
		private void DrawPagination()
		{
			if (m_editorVars.TotalPages == -1)
			{
				DeterminePages();
			}

			GUILayout.BeginHorizontal(GUILayout.Width(60));
			//Back button 
			if (GUILayout.Button("<", GUILayout.Height(24), GUILayout.Width(26)))
			{
				BackButtonPressed();
			}

			//Pages Label
			GUILayout.BeginHorizontal(EditorConstants.BoxStyle);
			GUILayout.Label(string.Format("Page {0} of {1}", m_editorVars.CurrentPage, m_editorVars.TotalPages));
			GUILayout.EndHorizontal();

			//Forward button
			if (GUILayout.Button(">", GUILayout.Height(24), GUILayout.Width(26)))
			{
				ForwardButtonPressed();
			}

			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// If window is in a broken state, notify the user and avoid null refs!
		/// </summary>
		private bool ErrorTestsPass()
		{
			if (m_so == null)
			{
				GUILayout.Label("SerializedObject not found!", EditorConstants.ErrorTextStyle);
				GUILayout.Label("Please reopen this window!", EditorConstants.ErrorTextStyle);
				return false;
			}
			if (m_elementsList == null)
			{
				GUILayout.Label("Cannot find elements list!", EditorConstants.ErrorTextStyle);
				GUILayout.Label("Did you forget to give your DataElement class the [System.Serializable] attribute?",
					EditorConstants.ErrorTextStyle);
				return false;
			}

			return true;
		}

		#endregion

		#region Functionality
		//============================================================================================================//
		// Functionality
		//============================================================================================================//
		/// <summary>
		/// Figure out TotalPages of elements.
		/// If the CurrentPage is out of bounds, reset it.
		/// </summary>
		private void DeterminePages()
		{
			bool hasRemainder = m_elementsList.arraySize % m_editorVars.ElementsPerPage != 0;
			m_editorVars.TotalPages = m_elementsList.arraySize / m_editorVars.ElementsPerPage + (hasRemainder ? 1 : 0);
			if (m_editorVars.TotalPages < 1) m_editorVars.TotalPages = 1;

			//If current page is out of bounds, set it back to page 1.
			if (m_editorVars.CurrentPage < 1 || m_editorVars.CurrentPage > m_editorVars.TotalPages)
			{
				SetCurrentPage(1);
			}
		}

		/// <summary>
		/// When an element is selected from the reorderable list...
		/// - Determine what page the element is on.
		/// - Set the current page to the element's page.
		/// - Set the element to be focused on the next frame.
		/// </summary>
		private void OnElementSelect(int index)
		{
			index += m_editorVars.ElementsPerPage;
			int quotient = index / m_editorVars.ElementsPerPage;
			int remainder = index % m_editorVars.ElementsPerPage;
			SetCurrentPage(quotient);
			SetElementToFocus(remainder);
		}

		/// <summary>
		/// If there is an element to focus on, focus on it.
		/// </summary>
		private void TryFocusElement()
		{
			if (m_elementToFocus == -1) return;

			if (Event.current.type == EventType.Repaint)
			{
				m_editorVars.RightScrollPos.y = m_listViewRectYPositions[m_elementToFocus];
				m_elementToFocus = -1;
				Repaint();
			}
		}

		/// <summary>
		/// When the back button is pressed
		/// - Move CurrentPage back one.
		/// - Reset scroll position;
		/// </summary>
		private void BackButtonPressed()
		{
			if (m_editorVars.CurrentPage > 1)
			{
				m_editorVars.CurrentPage--;
				m_editorVars.RightScrollPos.y = 0;
			}
		}

		/// <summary>
		/// When the forward button is pressed
		/// - Move CurrentPage forward one.
		/// - Reset scroll position;
		/// </summary>
		private void ForwardButtonPressed()
		{
			if (m_editorVars.CurrentPage < m_editorVars.TotalPages)
			{
				m_editorVars.CurrentPage++;
				m_editorVars.RightScrollPos.y = 0;
			}
		}
		#endregion

		#region Utility
		//============================================================================================================//
		// Utility
		//============================================================================================================//
		/// <summary>
		/// Sets the element to be focused on next frame.
		/// </summary>
		/// <param name="index">Index of element relative to the current page of elements displayed</param>
		private void SetElementToFocus(int index)
		{
			m_elementToFocus = index;
		}

		/// <summary>
		/// Simple setter for better readability.
		/// </summary>
		private void SetCurrentPage(int pageNum)
		{
			m_editorVars.CurrentPage = pageNum;
		}

		/// <summary>
		/// Get the next ID and then increment it.
		/// </summary>
		private int GetNextID()
		{
			return m_so.FindProperty(NEXTID_PATH).intValue++;
		}

		/// <summary>
		/// Save out the local instance of EditorVariables to the serialized object.
		/// </summary>
		private void SerializeEditorVars()
		{
			if (m_so == null || m_elementsList == null || m_editorVars == null) return;
			SerializedProperty editorVarsSP = m_so.FindProperty(EDITOR_VARIABLES_PATH);
			editorVarsSP.FindPropertyRelative(RELATIVE_LEFT_PANEL_RATIO).floatValue = m_splitterState.relativeSizes[0];
			editorVarsSP.FindPropertyRelative(SHOW_ASSET_INSPECTOR).boolValue = m_showAssetInspector;
			editorVarsSP.FindPropertyRelative(RELATIVE_ASSET_INSPECTOR_SIZE).floatValue = m_assetInspectorSplitterState.relativeSizes[1];
			editorVarsSP.FindPropertyRelative(LEFT_SCROLL_POS).vector2Value = m_editorVars.LeftScrollPos;
			editorVarsSP.FindPropertyRelative(RIGHT_SCROLL_POS).vector2Value = m_editorVars.RightScrollPos;
			editorVarsSP.FindPropertyRelative(ASSET_INSPECTOR_SCROLL_POS).vector2Value = m_editorVars.AssetInspectorScrollPos;
			editorVarsSP.FindPropertyRelative(ELEMENTS_PER_PAGE).intValue = m_editorVars.ElementsPerPage;
			editorVarsSP.FindPropertyRelative(CURRENT_PAGE).intValue = m_editorVars.CurrentPage;
			editorVarsSP.FindPropertyRelative(TOTAL_PAGES).intValue = m_editorVars.TotalPages;

			m_so.ApplyModifiedProperties();
		}

		/// <summary>
		/// Get the EditorVariables from the serialized object.
		/// </summary>
		private void DeserializeEditorVars()
		{
			if (m_so == null) return;
			SerializedProperty editorVarsSP = m_so.FindProperty(EDITOR_VARIABLES_PATH);
			m_editorVars = new EditorVariables();
			m_splitterState.relativeSizes[0] = editorVarsSP.FindPropertyRelative(RELATIVE_LEFT_PANEL_RATIO).floatValue;
			m_splitterState.relativeSizes[1] = 1 - m_splitterState.relativeSizes[0];
			m_showAssetInspector = editorVarsSP.FindPropertyRelative(SHOW_ASSET_INSPECTOR).boolValue;
			m_assetInspectorSplitterState.relativeSizes[1] = editorVarsSP.FindPropertyRelative(RELATIVE_ASSET_INSPECTOR_SIZE).floatValue;
			m_assetInspectorSplitterState.relativeSizes[0] = 1 - m_assetInspectorSplitterState.relativeSizes[1];
			m_editorVars.LeftScrollPos = editorVarsSP.FindPropertyRelative(LEFT_SCROLL_POS).vector2Value;
			m_editorVars.RightScrollPos = editorVarsSP.FindPropertyRelative(RIGHT_SCROLL_POS).vector2Value;
			m_editorVars.AssetInspectorScrollPos = editorVarsSP.FindPropertyRelative(ASSET_INSPECTOR_SCROLL_POS).vector2Value;
			m_editorVars.ElementsPerPage = editorVarsSP.FindPropertyRelative(ELEMENTS_PER_PAGE).intValue;
			m_editorVars.CurrentPage = editorVarsSP.FindPropertyRelative(CURRENT_PAGE).intValue;
			m_editorVars.TotalPages = editorVarsSP.FindPropertyRelative(TOTAL_PAGES).intValue;
		}

		private void CheckForEditorVarsChanges()
		{
			SerializedProperty editorVarsSP = m_so.FindProperty(EDITOR_VARIABLES_PATH);
			if (editorVarsSP.FindPropertyRelative(ELEMENTS_PER_PAGE).intValue != m_editorVars.ElementsPerPage)
			{
				m_editorVars.ElementsPerPage = editorVarsSP.FindPropertyRelative(ELEMENTS_PER_PAGE).intValue;
				m_editorVars.CurrentPage = editorVarsSP.FindPropertyRelative(CURRENT_PAGE).intValue;
				m_editorVars.TotalPages = editorVarsSP.FindPropertyRelative(TOTAL_PAGES).intValue;
			}
		}

		[OnOpenAsset(0)]
		public static bool OnDatastoreAssetClicked(int instanceID, int line)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceID);
			if (!(obj is BaseDatastore))
			{
				return false;
			}

			Init(AssetDatabase.GetAssetPath(obj));
			return true;
		}
		#endregion
	}
}