using Cafeo.Data;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class WorldShopTemplate : WithDisplayName
    {
        [BoxGroup("Naming", centerLabel: true)]
        public string[] names;
        [BoxGroup("Economy", centerLabel: true)] [EnumToggleButtons]
        public WorldItem.ItemPool itemPool;
    }
}