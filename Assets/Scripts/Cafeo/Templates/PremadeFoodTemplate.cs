using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class PremadeFoodTemplate : WorldItemTemplate
    {
        [BoxGroup("Food Attributes", centerLabel: true)]
        public int satiation;
        [BoxGroup("Food Attributes", centerLabel: true)]
        public int taste;

        public enum VariantTypes
        {
            None,
            SmallLargeVariant,
            ThreeSizeVariant,
        }
    }
}