using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class SoulTemplate : WithIcon, IComponentTemplate<AgentSoul>
    {
        [BoxGroup("Basic Information", centerLabel: true)]
        public AgentSoul.Gender gender;

        [BoxGroup("Basic Information", centerLabel: true)]
        public string firstName;

        [BoxGroup("Basic Information", centerLabel: true)]
        public string lastName;

        [BoxGroup("Basic Information", centerLabel: true)]
        public int age = 17;

        [BoxGroup("Physique", centerLabel: true)]
        public float heightZ;

        [BoxGroup("Physique", centerLabel: true)]
        public float bmi = 22.5f;

        [BoxGroup("Physique", centerLabel: true)]
        public float muscleStrength = 1;

        [BoxGroup("Physique", centerLabel: true)]
        public float limbSize = 1;

        [BoxGroup("Modifiers", centerLabel: true)]
        public float sizeMultiplier = 1;

        [BoxGroup("Modifiers", centerLabel: true)]
        public float appearedAge = 1f;

        [BoxGroup("Stats", centerLabel: true)] public BaseAttrs baseAttrs;

        [BoxGroup("Battle Setup", centerLabel: true)] [AssetSelector]
        public JobTemplate job;

        [BoxGroup("Battle Setup", centerLabel: true)] [AssetSelector]
        public WeaponTypeTemplate mainWeapon;

        [BoxGroup("Battle Setup", centerLabel: true)] [AssetSelector]
        public WeaponTypeTemplate secondaryWeapon;

        public virtual AgentSoul AddToGameObjet(GameObject gameObject)
        {
            var soul = gameObject.AddComponent<AgentSoul>();
            soul.firstName = firstName;
            soul.lastName = lastName;
            soul.age = age;
            soul.heightZ = heightZ;
            soul.bmi = bmi;
            soul.muscleStrength = muscleStrength;
            soul.limbSize = limbSize;
            soul.sizeMultiplier = sizeMultiplier;
            soul.appearedAge = appearedAge;
            baseAttrs.CopyTo(soul.baseAttrs);
            return soul;
        }

        public virtual void ModifyVessel(BattleVessel vessel)
        {
        }
    }
}