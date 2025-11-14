using System;
using System.Linq;
using Tooltips.Manipulators;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Tooltips
{
	[DefaultExecutionOrder(1)]
    public class WorldTooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TypeSelector, SerializeReference] private TooltipInfoSource tooltipSource;

        public StaticTooltipTracking tracking = new StaticTooltipTracking(new WorldObjectAnchor(null,null));

        private TooltipElement       _tooltip;
        public TooltipElement VisualElement => _tooltip; 
        public void SetTooltipInfo(TooltipInfoSource tooltipInfoSource)
        {
	        tooltipSource = tooltipInfoSource;
	        if (!_tooltip.ClassListContains("transition"))
	        {
		        _tooltip.SetTooltipInfo(tooltipSource.GetTooltipInfo());
	        }

	        foreach (var element in _tooltip.Query<TextElement>().Build())
	        {
				SetLanguageClass(element,ScribeManager.Active.FontName);
	        }
        }

        private void SetLanguageClass(TextElement text, string languageCode)
        {
	        var activeLanguage = text.GetClasses().Where(x => x.Contains("lang-")).ToArray();
	        foreach (string s in activeLanguage)
	        {
		        text.RemoveFromClassList(s);
	        }
	        text.AddToClassList($"lang-{languageCode}");
        }
        
        private void Awake()
        {
	        _tooltip = new TooltipElement();
	        _tooltip.AddToClassList("transition");
	        TooltipLayer.Add(_tooltip);
        }

        void Start()
        {
            tracking.Anchor ??= new WorldObjectAnchor(transform, null);
            if (tracking.Anchor is ITooltipAnchorWithCamera withCamera)
            {
	            withCamera.Cam = Camera.main;
            }
            _tooltip.SetTooltipInfo(tooltipSource.GetTooltipInfo());
            _tooltip.Hide();
        }

        private void OnDestroy()
        {
            TooltipLayer.Remove(_tooltip);
        }

        void Update()
        {
            // recompute screen pos every frame
            if (_tooltip != null && _tooltip.IsVisible && _tooltip.panel != null)
            {
                _tooltip?.SetAnchorPosition(tracking);
            }
        }

        void OnDisable()
        {
            _tooltip?.Hide();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Pointer Enter at:{name}");
            Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Pointer Exit at:{name}");
            Hide();
        }

        public async void Show()
        {
	        try
	        {
		        await _tooltip.ShowAsync(destroyCancellationToken);
	        }
	        catch (Exception e)
	        {
		        if (e is not OperationCanceledException)
		        {
			        Debug.LogException(e);
		        }
	        }
        }
       
        
        public async void Hide()
        {
	        try
	        {
		        await _tooltip.HideAsync(destroyCancellationToken);
	        }
	        catch (Exception e)
	        {
		        if (e is not OperationCanceledException)
		        {
			        Debug.LogException(e);
		        }
	        }
        }
    }
}