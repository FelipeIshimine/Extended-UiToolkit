using Core.Data.Tooltips;

namespace Tooltips.Manipulators
{
    [System.Serializable]
    public abstract class TooltipInfoSource
    {
        public abstract TooltipData GetTooltipInfo();

        [System.Serializable]
        public class Instance : TooltipInfoSource
        {
            public TooltipData value;
            public Instance(){}
            public Instance(TooltipData value)
            {
                this.value = value;
            }
            public override TooltipData GetTooltipInfo() => value;
        }
    }
}