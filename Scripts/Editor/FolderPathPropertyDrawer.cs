using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ExtendedUiToolkit.Editor
{
	[CustomPropertyDrawer(typeof(FolderPathAttribute))]
	public class FolderPathPropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// Create the root element
			var root = new VisualElement();

			// Create an instance of your AutocompleteTextField
			var autocompleteField = new AutocompleteTextField();
        
			// Set the initial value to the property's current value
			autocompleteField.TextField.value = property.stringValue;

			// Populate the suggestions with folder paths in the Assets folder
			autocompleteField.SetSuggestions(GetAllAssetFolders());

			// Register change event to update the property when a new folder is selected
			autocompleteField.TextField.BindProperty(property);

			root.Add(autocompleteField);
			return root;
		}

		// Helper function to get all folders in the Assets directory
		private static string[] GetAllAssetFolders()
		{
			string[] directories = Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories);
			return directories.Select(d => d.Replace("\\", "/")).ToArray(); // Normalize paths
		}
	}
}
