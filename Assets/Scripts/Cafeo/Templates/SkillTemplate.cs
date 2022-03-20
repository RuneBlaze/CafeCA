using System;
using Cafeo.Castable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class SkillTemplate : WithIcon
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
        }
        
    }
}