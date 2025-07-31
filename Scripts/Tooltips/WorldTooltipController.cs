using System;
using Tooltips.Manipulators;
using TypeSelector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tooltips
{
    public class WorldTooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TypeSelector, SerializeReference] private TooltipInfoSource tooltipSource;

        public StaticTooltipTracking tracking = new StaticTooltipTracking(new WorldObjectAnchor(null,null));

        private TooltipElement       _tooltip;

        void Start()
        {
            _tooltip = new TooltipElement();
            TooltipLayer.Add(_tooltip);
            tracking.Anchor ??= new WorldObjectAnchor(transform, Camera.main);
            _tooltip.SetTooltipInfo(tooltipSource.GetTooltipInfo());
        }

        private void OnDestroy()
        {
            TooltipLayer.Remove(_tooltip);
        }

        void Update()
        {
            // recompute screen pos every frame
            if (_tooltip != null && _tooltip.IsVisible)
            {
                _tooltip?.SetAnchorPosition(tracking);
            }
        }

        void OnDisable()
        {
            _tooltip?.Hide();
        }
        public async void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Pointer Enter at:{name}");
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

        public async void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Pointer Exit at:{name}");
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