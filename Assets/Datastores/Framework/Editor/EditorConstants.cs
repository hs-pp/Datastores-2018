using UnityEngine;

namespace Datastores.Framework.Editor
{
	/// <summary>
	/// Constants to be used in all editor related scripts.
	/// </summary>
	public static class EditorConstants
	{
		//============================================================================================================//
		// Used in DatastoreWindow.cs
		//============================================================================================================//
		public static GUIStyle BoxStyle;
		public static GUIStyle DatastoreTitleStyle;
		public static GUIStyle TinyTextStyle;
		public static GUIStyle TextFieldPlaceHolderStyle;
		public static GUIStyle ErrorTextStyle;
		
		//============================================================================================================//
		// Used in DatastoreLibrary.cs
		//============================================================================================================//
		public static GUIStyle LibraryTypeStyle;
		public static GUIStyle LibraryDatastoreNameStyle;
		
		//============================================================================================================//
		// Used in DataElementHelper.cs when drawing the OnGUI default values.
		//============================================================================================================//
		public static GUIStyle DataElementNameStyle;
		public static GUIStyle DataElementIDStyle;
		
		static EditorConstants()
		{
			BoxStyle = new GUIStyle("Box");
			
			DatastoreTitleStyle = new GUIStyle();
			DatastoreTitleStyle.fontSize = 25;
			DatastoreTitleStyle.fontStyle = FontStyle.Bold;
			
			TinyTextStyle = new GUIStyle();
			TinyTextStyle.fontSize = 11;
			TinyTextStyle.fontStyle = FontStyle.Bold;
			
			TextFieldPlaceHolderStyle = new GUIStyle();
			TextFieldPlaceHolderStyle.fontSize = 10;
			TextFieldPlaceHolderStyle.fontStyle = FontStyle.Italic;
			TextFieldPlaceHolderStyle.normal.textColor = Color.grey;

			ErrorTextStyle = new GUIStyle();
			ErrorTextStyle.fontSize = 18;
			ErrorTextStyle.fontStyle = FontStyle.Bold;
			ErrorTextStyle.normal.textColor = Color.red;
			
			
			LibraryTypeStyle = new GUIStyle();
			LibraryTypeStyle.fontSize = 20;
			LibraryTypeStyle.fontStyle = FontStyle.Bold;

			LibraryDatastoreNameStyle = new GUIStyle();
			LibraryDatastoreNameStyle.fontSize = 14;
			LibraryDatastoreNameStyle.fontStyle = FontStyle.Bold;

			DataElementNameStyle = new GUIStyle();
			DataElementNameStyle.fontSize = 14;
			
			DataElementIDStyle = new GUIStyle();
			DataElementIDStyle.fontSize = 9;
			DataElementIDStyle.fontStyle = FontStyle.Italic;
		}
	}
}