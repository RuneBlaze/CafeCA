using Cafeo.Data;
using Sirenix.OdinInspector;

namespace Cafeo.Templates
{
    public class WorldItemTemplate : WithIcon
    {
        [BoxGroup("Economy", centerLabel: true)] [EnumToggleButtons]
        public WorldItem.ItemPool itemPool;
    }
}