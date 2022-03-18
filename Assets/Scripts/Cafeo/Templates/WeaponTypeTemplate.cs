using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class WeaponTypeTemplate : WithDisplayName
    {
        [BoxGroup("Weapon Specs", centerLabel: true)]
        public SkillTreeTemplate skillTree;

        [BoxGroup("Weapon Specs", centerLabel: true)]
        public float sizeModifier;
    }
}