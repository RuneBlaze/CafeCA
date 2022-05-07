using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Castable;
using Cafeo.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public abstract class SkillTemplate : WithIcon, ITemplate<UsableItem>
    {
        // [BoxGroup("Basic Info", centerLabel: true)]
        // public string displayName;
        [BoxGroup("Basic Info", centerLabel: true)]
        [HideLabel, TextArea(4, 14)]
        public string description;

        [BoxGroup("Basic Info", centerLabel: true)]
        public int mpCost;
        [BoxGroup("Basic Info", centerLabel: true)]
        public int cpCost;
        
        [BoxGroup("Effects", centerLabel: true)]
        public float power = 100f;

        [BoxGroup("Effects", centerLabel: true)]
        public float knockbackPower = 1f;

        [BoxGroup("Effects", centerLabel: true)]
        public UsableItem.PowerType powerType;
        [BoxGroup("Effects", centerLabel: true)]
        public UsableItem.DamageType damageType;
        // [BoxGroup("Effects", centerLabel: true)]
        // public bool isArts = false;
        [BoxGroup("Effects", centerLabel: true)]
        public TemplateHitType hitType = TemplateHitType.HitEnemies;
        [BoxGroup("Effects", centerLabel: true)]
        public float hitStun = 0.2f;
        [BoxGroup("Effects", centerLabel: true)]
        public TemplateSkillTimeline timePoints;
        [BoxGroup("Effects", centerLabel: true)]
        public bool stopOnUse = false;
        [BoxGroup("Effects", centerLabel: true)]
        public HitEffects hitEffects;

        [BoxGroup("Advanced", centerLabel: true)]
        public int orbit = 1;

        [BoxGroup("AI Settings", centerLabel: true)]
        public List<UsableItem.ItemTag> tags;

        [BoxGroup("AI Settings", centerLabel: true)]
        public UtilitySetting utilitySetting;
        
        public enum TemplateHitType
        {
            HitNone,
            HitAllies,
            HitEnemies,
            HitBoth,
        }
        [Serializable]
        public struct TemplateSkillTimeline
        {
            public float startUp;
            public float active;
            public float recovery;
        }

        public enum UtilityTypePresets
        {
            SingleEnemyInRange,
            SingleEnemyInDirection,
            Cooldown,
            PenalizeDanger,
        }

        [Serializable]
        public class UtilityTypeTemplate
        {
            public UtilityTypePresets preset;
            public Vector4 userData = Vector4.one;
            public float multiplier = 1;
        }

        [Serializable]
        public class UtilitySetting : ITemplate<UtilityType>
        {
            public List<UtilityTypeTemplate> utilityTypes;
            public float constantAdder;
            public float constantMultiplier = 1;
            public UtilityType Generate()
            {
                var utilities = utilityTypes.Select(it =>
                {
                    var ud = it.userData;
                    UtilityType baseUtility = it.preset switch
                    {
                        UtilityTypePresets.SingleEnemyInDirection => new UtilityType.SingleEnemyInDirection(),
                        UtilityTypePresets.SingleEnemyInRange => new UtilityType.SingleEnemyInRange(ud.x),
                        UtilityTypePresets.Cooldown => new UtilityType.Cooldown(ud.x),
                        UtilityTypePresets.PenalizeDanger => new UtilityType.PenalizeDanger(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    return baseUtility * it.multiplier;
                }).ToList();
                if (utilities.Count == 0)
                {
                    return (UtilityType)10f;
                }
                UtilityType agg = utilities[0];
                bool firstOne = true;
                foreach (var utilityType in utilities)
                {
                    if (firstOne)
                    {
                        agg = utilityType;
                        firstOne = false;
                    }
                    else
                    {
                        agg += utilityType;
                    }
                }
                if (Math.Abs(constantMultiplier - 1) > 0.01f)
                {
                    return agg * constantMultiplier + (UtilityType)constantAdder;
                }
                return agg;
            }

            public UtilitySetting()
            {
                utilityTypes = new List<UtilityTypeTemplate>();
            }
        }
        
        protected void CopyBaseParameters(UsableItem item)
        {
            item.name = displayName;
            item.description = description;
            item.icon = icon;
            item.power = power;
            item.powerType = powerType;
            item.damageType = damageType;
            switch (hitType)
            {
                case TemplateHitType.HitAllies:
                    item.SetHitAllies();
                    break;
                case TemplateHitType.HitEnemies:
                    item.SetHitEnemies();
                    break;
                case TemplateHitType.HitBoth:
                    item.hitAllies = true;
                    item.hitEnemies = true;
                    break;
                case TemplateHitType.HitNone:
                    item.hitAllies = false;
                    item.hitEnemies = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            item.hitStun = hitStun;
            item.startUp = timePoints.startUp;
            item.active = timePoints.active;
            item.recovery = timePoints.recovery;
            item.stopOnUse = stopOnUse;
            item.hitEffects = hitEffects;
            item.orbit = orbit;
            item.tags = tags;
            item.knockbackPower = knockbackPower;
            item.utilityType = utilitySetting.Generate();
        }

        private void Reset()
        {
            tags.Clear();
            tags.Add(UsableItem.ItemTag.FreeDPS);
            utilitySetting.utilityTypes.Clear();
            utilitySetting.utilityTypes.Add(new UtilityTypeTemplate {preset = UtilityTypePresets.SingleEnemyInDirection});
            timePoints.active = 0.3f;
            timePoints.recovery = 0.05f;
        }

        public abstract UsableItem Generate();
    }
}