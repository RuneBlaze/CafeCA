using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Data;
using Cafeo.Templates;
using Cafeo.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo
{
    public class AgentSoul : MonoBehaviour
    {
        public enum Gender
        {
            Male,
            Female
        }

        public enum ResourceType
        {
            Hp,
            Mp,
            Cp
        }

        public string firstName;
        public string lastName;
        public int age = 17;
        public Gender gender = Gender.Male;
        public float heightZ;
        public float bmi = 22.5f;
        public float muscleStrength = 1;
        public float limbSize = 1;
        public float sizeMultiplier = 1;
        public float appearedAge = 1f;
        public string skinColor = "fair";
        public string hairColor = "black";
        public string eyeColor = "brown";
        public float genderAppearance = 0.7f;
        public bool hasMaleFeature;
        public bool hasFemaleFeature;
        public bool hasBreasts;
        public float carved = 0.5f;
        public float maleFeatureProminence = 1f;
        public float femaleFeatureProminence = 1f;

        [SerializeField] private AgentPreset preset;

        public int hp = 12;
        public int mp = 6;
        public int cp;

        public float[] baseAttrs = new float[12];

        public int alignment;

        public Dictionary<WearableTemplate.GarmentPosition, List<Wearable>> clothes;

        public string DisplayName
        {
            get
            {
                if (firstName == "" && lastName == "") return gameObject.name;
                return $"{lastName} {firstName}";
            }
        }

        public float Str => baseAttrs[0] * Mathf.Pow(HeightScore, 1.5f);
        public float Con => baseAttrs[1] * Mathf.Pow(HeightScore, 1.5f);
        public float Dex => baseAttrs[2] / Mathf.Pow(HeightScore, 1.5f);
        public float Per => baseAttrs[3];
        public float Lea => baseAttrs[4];
        public float Wil => baseAttrs[5];
        public float Mag => baseAttrs[6];
        public float Cut => baseAttrs[7];
        public float Awe => baseAttrs[8];
        public float Life => baseAttrs[9];
        public float Mana => baseAttrs[10];

        [ShowInInspector] public float Atk => Str * 2 + Con * 0.5f;

        [ShowInInspector] public float Def => Con * 2 + Str * 0.5f;

        [ShowInInspector] public float Mat => Mag * 2 + Wil * 0.5f;

        [ShowInInspector] public float Mdf => Wil * 2 + Mag * 0.5f;

        [ShowInInspector]
        public int MaxHp =>
            Mathf.RoundToInt((5 + Life / 100 * (Con * 3 + Str + Wil) / 3) * 20 * Mathf.Pow(HeightScore, 0.8f));

        [ShowInInspector] public int MaxMp => Mathf.RoundToInt(Mana * (Mag + (6 * Mag + 3 * Wil + Lea) / 10) / 5);

        public int MaxCp => 200;

        public float RawHeight
        {
            get
            {
                var hd = HeightData.Instance;
                return hd.GetHeight(gender, ApparentAge, heightZ);
            }
        }

        public float ApparentAge => age * appearedAge;
        public int GirlInd => gender == Gender.Female ? 1 : 0;
        public int BoyInd => 1 - GirlInd;

        [ShowInInspector] public float Height => RawHeight * sizeMultiplier;

        public float HeightScore => Height / 160;

        [ShowInInspector] public float Weight => bmi * Mathf.Pow(Height / 100f, 2);


        public float FootLength
        {
            get
            {
                var a = ApparentAge;
                var h = Height;
                return h * Mathf.Lerp(0.165f, 0.15f, Mathf.Clamp(a, 3, 16) / 16);
            }
        }

        public float HandLength
        {
            get
            {
                var r = (Height - 75.31f) / 4.782f;
                return r * Mathf.Clamp(limbSize, 0.8f, 1.2f);
            }
        }

        public int ShoeSize => Mathf.RoundToInt(FootLength * 2 - 10);

        public bool Dead => hp <= 0;

        private void Awake()
        {
            if (preset != null) ConsumePreset();

            InitClothes();
        }

        private void Start()
        {
            hp = MaxHp;
            mp = MaxMp;
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void CopyTo(AgentSoul dst)
        {
            dst.firstName = firstName;
            dst.lastName = lastName;
            dst.age = age;
            dst.gender = gender;
            dst.muscleStrength = muscleStrength;
            dst.limbSize = limbSize;
            dst.sizeMultiplier = sizeMultiplier;
            dst.appearedAge = appearedAge;
            dst.baseAttrs = baseAttrs.AsEnumerable().ToArray();
        }

        public void TakeDamage(int k, ResourceType t = ResourceType.Hp)
        {
            switch (t)
            {
                case ResourceType.Hp:
                    k = Mathf.Clamp(k, 0, hp);
                    hp -= k;
                    break;
                case ResourceType.Mp:
                    k = Mathf.Clamp(k, 0, mp);
                    mp -= k;
                    break;
                case ResourceType.Cp:
                    k = Mathf.Clamp(k, 0, cp);
                    cp -= k;
                    break;
            }
        }

        public void Heal(int k)
        {
            k = Mathf.Clamp(k, 0, MaxHp - hp);
            hp += k;
        }

        public void HealPercentage(float perc)
        {
            Heal(Mathf.RoundToInt(MaxHp * perc));
        }

        public void HealMp(int k)
        {
            k = Mathf.Clamp(k, 0, MaxMp - mp);
            mp += k;
        }

        public void HealMpPercentage(float perc)
        {
            HealMp(Mathf.RoundToInt(MaxMp * perc));
        }

        public void HealCp(int k)
        {
            k = Mathf.Clamp(k, 0, MaxCp - cp);
            cp += k;
        }

        public void HealCpPercentage(float perc)
        {
            HealCp(Mathf.RoundToInt(MaxCp * perc));
        }

        private void InitClothes()
        {
            clothes = new Dictionary<WearableTemplate.GarmentPosition, List<Wearable>>();
            foreach (var value in Enum.GetValues(typeof(WearableTemplate.GarmentPosition)))
            {
                var pos = (WearableTemplate.GarmentPosition)value;
                clothes[pos] = new List<Wearable>();
            }
        }

        public bool PutOn(Wearable wearable)
        {
            var layer = wearable.Layer;
            var position = wearable.Position;
            var positions = new List<WearableTemplate.GarmentPosition>();
            foreach (var value in Enum.GetValues(typeof(WearableTemplate.GarmentPosition)))
            {
                var pos = (WearableTemplate.GarmentPosition)value;
                if (position.HasFlag(pos)) positions.Add(pos);
            }

            foreach (var garmentPosition in positions)
            {
                var listOfClothes = clothes[garmentPosition];
                if (listOfClothes[^1].Layer >= layer) return false;
            }

            foreach (var garmentPosition in positions)
            {
                var listOfClothes = clothes[garmentPosition];
                listOfClothes.Add(wearable);
            }

            return true;
        }

        [Button("Consume Preset")]
        private void ConsumePreset()
        {
            if (preset == null) return;
            baseAttrs[0] = preset.str;
            baseAttrs[1] = preset.con;
            baseAttrs[2] = preset.dex;
            baseAttrs[3] = preset.per;
            baseAttrs[4] = preset.lea;
            baseAttrs[5] = preset.wil;
            baseAttrs[6] = preset.mag;
            baseAttrs[7] = preset.cut;
            baseAttrs[8] = preset.awe;
            baseAttrs[9] = preset.life;
            baseAttrs[10] = preset.mana;
        }

        public void Revive()
        {
            hp = MaxHp;
            mp = MaxMp;
            cp = 0;
        }
    }
}