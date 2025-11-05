using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(UIDocument))]
public class TooltipLayer : MonoBehaviour
{
    private static TooltipLayer Instance { get; set; }

    [SerializeField] private UIDocument document;
    private VisualElement _root;

    private void Reset() => document = GetComponent<UIDocument>();

    protected void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Singleton initialization failed", this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
        document.visualTreeAsset = ScriptableObject.CreateInstance<VisualTreeAsset>();
        _root = document.rootVisualElement;
    }

    protected void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Destroy(document.visualTreeAsset);
        }
    }


    public static void Add(VisualElement tooltipContainer)
    {
        //Debug.Log("ADD");
        Instance._root.Add(tooltipContainer);
    }

    public static void Remove(VisualElement element)
    {
        //Debug.Log("REMOVE");
        if (Instance)
        {
            Instance._root.Remove(element);
        }
    }

}