using System;
using Cafeo.Castable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class SkillTemplate : WithIcon, ITemplate<UsableItem>
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

        public virtual UsableItem Create()
        {
            throw new NotImplementedException();
        }
    }
}