using System;
using System.IO;

namespace ExtendedUiToolkit.Editor
{
	public static class FolderUtility
	{
		/// <summary>
		/// Checks if the specified folder path exists. If not, creates the folder.
		/// </summary>
		/// <param name="folderPath">The path of the folder to check and create.</param>
		public static void EnsureFolderExists(string folderPath)
		{
			// Ensure the folder path is valid
			if (string.IsNullOrWhiteSpace(folderPath))
			{
				throw new ArgumentException("Folder path cannot be null or empty.", nameof(folderPath));
			}

			// Convert to absolute path if needed (this example assumes a relative path)
			string absolutePath = Path.GetFullPath(folderPath);

			// Check if the directory exists
			if (!Directory.Exists(absolutePath))
			{
				// Create the directory
				Directory.CreateDirectory(absolutePath);
				UnityEngine.Debug.Log($"Folder created at: {absolutePath}");
			}
			else
			{
				UnityEngine.Debug.Log($"Folder already exists at: {absolutePath}");
			}
		}
	}
}