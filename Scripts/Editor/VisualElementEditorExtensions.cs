using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public static class VisualElementEditorExtensions
{
    public static void DrawDefaultEditor(this VisualElement element, SerializedObject serializedObject, bool skipScriptField = false)
    {
        var iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            if (skipScriptField)
            {
                while (iterator.NextVisible(false))
                {
                    element.Add(new PropertyField(iterator));
                }    
            }
            else
            {
                do
                {
                    element.Add(new PropertyField(iterator));
                } while (iterator.NextVisible(false));    
            }
        }
    }

}