using System;
using System.Collections.Generic;
using Cafeo.Templates;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Cafeo.Configs
{
    [GlobalConfig("Assets/Resources/Configs/")]
    public class InitialParty : GlobalConfig<InitialParty>
    {
        [AssetsOnly] public List<SoulTemplate> initialMembers;

        public List<BattleInventoryConfig> battleInventories;

        [Serializable]
        public class BattleInventoryConfig
        {
            [AssetSelector] public TreasureTemplate treasure;
            [AssetSelector] public List<OneTimeUseTemplate> oneTimeUses;
            [AssetSelector] public List<SkillTemplate> restSkills;
        }
    }
}