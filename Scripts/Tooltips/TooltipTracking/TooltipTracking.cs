using UnityEngine;

namespace Tooltips
{
    [System.Serializable]
    public abstract class TooltipTracking
    {
        protected TooltipTracking()
        {
        }

        public abstract void ApplyTo(TooltipElement tooltipElement);
        
        public static Vector2 CalculateTranslationDelta(TooltipPlacement placement)
        {
            return placement switch
            {
                TooltipPlacement.Top           => new Vector2(-50, -100),
                TooltipPlacement.Bottom        => new Vector2(-50, 0),
                TooltipPlacement.Left          => new Vector2(-100, -50),
                TooltipPlacement.Right         => new Vector2(0, -50),
                TooltipPlacement.TopLeft       => new Vector2(-100, -100),
                TooltipPlacement.TopRight      => new Vector2(0, -100),
                TooltipPlacement.BottomLeft    => new Vector2(-100, 0),
                TooltipPlacement.BottomRight   => new Vector2(0, 0),
                TooltipPlacement.Center        => new Vector2(-50, -50),
                _                               => Vector2.zero
            };
        }
               
        public static Vector2 GetPlacementUnitVector(TooltipPlacement p) => p switch
        {
            TooltipPlacement.Top           => new Vector2( 0, -1),
            TooltipPlacement.Bottom        => new Vector2( 0,  1),
            TooltipPlacement.Left          => new Vector2(-1,  0),
            TooltipPlacement.Right         => new Vector2( 1,  0),
            TooltipPlacement.TopLeft       => new Vector2(-.7071f, - .7071f),
            TooltipPlacement.TopRight      => new Vector2(.7071f,  - .7071f),
            TooltipPlacement.BottomLeft    => new Vector2(-.7071f,  .7071f),
            TooltipPlacement.BottomRight   => new Vector2(.7071f,   .7071f),
            TooltipPlacement.Center        => Vector2.zero,
            _                               => Vector2.zero
        };
    }
}