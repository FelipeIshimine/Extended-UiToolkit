using Core.Data.Tooltips;
using Tooltips;
using TypeSelector;
using UI.Manipulators;
using UnityEngine;
using UnityEngine.UIElements;

public class LinkFinderComponent : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    public TooltipData tooltip = new TooltipData()
    {
        Title = "Title goes here",
        Body = "This is the body of the tooltip, usually longer",
        Footer = "Some useful, or not so useful text"
    };

    [TypeSelector, SerializeReference] public DynamicTooltipTracking tracking;
    
    public void Awake()
    {
        document.rootVisualElement.Q<Label>().AddManipulator(new LinkFocusManipulator(LogEnter,LogExit));
        foreach (var element in document.rootVisualElement.Query<VisualElement>("Cosa").Build())
        {
            element.AddManipulator(
                new TooltipManipulator(
                    tooltip,
                    new DynamicTooltipTracking()
                    {
                        focusMode = tracking.focusMode,
                        Margin = tracking.Margin,
                        discretePosition = tracking.discretePosition
                    }));
        }
        
        
    }

    private void LogEnter(string obj)
    {
        Debug.Log($"ENTER:{obj}");
    }

    private void LogExit(string obj)
    {
        Debug.Log($"EXIT:{obj}");
    }
}