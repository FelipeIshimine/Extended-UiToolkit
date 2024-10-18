using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExtendedUI.Editor
{
	public class FolderSelectorElement : VisualElement
	{
		private string AssetsFolderPath => $"{Application.dataPath}";
		private SerializedProperty _serializedProperty;
		private AutocompleteTextField pathField;
		private Button _selectButton;
		public TextField TextField => pathField.TextField;
		
		//public new class UxmlFactory : UxmlFactory<FolderSelectorElement, UxmlTraits> { }

		public FolderSelectorElement(SerializedProperty serializedProperty)
		{
			this.style.flexDirection = FlexDirection.Row;
			_serializedProperty = serializedProperty;

		

			// Create and configure the button
			_selectButton = new Button(OnSelectButtonClicked)
			{
				name = "SelectFolderButton",
				text = string.Empty,
				style = 
				{
					backgroundImage = new StyleBackground((Texture2D)EditorGUIUtility.Load("Folder On Icon"))
				}
			};
			
			_selectButton.style.marginLeft =
				_selectButton.style.paddingLeft =
					_selectButton.style.marginRight =
						_selectButton.style.paddingRight =
							_selectButton.style.marginLeft =
								_selectButton.style.paddingLeft =
									_selectButton.style.marginRight =
										_selectButton.style.paddingRight = 0;

			_selectButton.style.width = 20;
			Add(_selectButton);

			Add(new Label("Assets/"));
			
			// Create and configure the label
			pathField = new AutocompleteTextField()
			{
				style = {flexGrow = 1 }
			};
			pathField.TextField.BindProperty(serializedProperty);
			
			pathField.SetSuggestions(GetAllAssetFolders());
			// Add the elements to the root of this custom element
			Add(pathField);

			// Register callback to update when the property changes
			serializedProperty.serializedObject.Update();
			this.TrackPropertyValue(serializedProperty,OnSerializedObjectModified);
		}

		private void OnSelectButtonClicked()
		{
			// Open the folder selection dialog
			string path = EditorUtility.OpenFolderPanel("Select Folder", $"{AssetsFolderPath}/{_serializedProperty.stringValue}", "");
			
			Debug.Log(AssetsFolderPath);
			if (!string.IsNullOrEmpty(path))
			{
				// Set the selected folder path and update the label
				_serializedProperty.stringValue = path.Replace(AssetsFolderPath, string.Empty);
				_serializedProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		private void OnSerializedObjectModified(SerializedProperty serializedProperty)
		{
			// Update the label when the serialized object changes
			_serializedProperty = serializedProperty;
			pathField.TextField.BindProperty(_serializedProperty);
		}
		
          // Helper function to get all folders in the Assets directory
                private static string[] GetAllAssetFolders()
                {
        	        string[] directories = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories);
        	        return directories.Select(d => d.Replace("\\", "/").Remove(0,7)).ToArray(); // Normalize paths
                }
	}
}