using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Female,
        }

        public string DisplayName
        {
            get
            {
                if (firstName == "" && lastName == "")
                {
                    return gameObject.name;
                }
                return $"{lastName} {firstName}";
            }
        }

        public string firstName;
        public string lastName;
        public int age = 17;
        public Gender gender = Gender.Male;
        public float heightZ = 0;
        public float bmi = 22.5f;
        public float muscleStrength = 1;
        public float limbSize = 1;
        public float sizeMultiplier = 1;
        public float appearedAge = 1f;
        public string skinColor = "fair";
        public string hairColor = "black";
        public string eyeColor = "brown";
        public float genderAppearance = 0.7f;
        public bool hasMaleFeature = false;
        public bool hasFemaleFeature = false;
        public bool hasBreasts = false;
        public float carved = 0.5f;
        public float maleFeatureProminence = 1f;
        public float femaleFeatureProminence = 1f;

        // public int gold;
        // public string aiType;

        [SerializeField] private AgentPreset preset;

        public int hp = 12;
        public int mp = 6;
        public int cp = 0;

        public float[] baseAttrs = new float[12];
        
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
        
        [ShowInInspector]
        public float Atk => Str * 2 + Con * 0.5f;
        [ShowInInspector]
        public float Def => Con * 2 + Str * 0.5f;
        [ShowInInspector]
        public float Mat => Mag * 2 + Wil * 0.5f;
        [ShowInInspector]
        public float Mdf => Wil * 2 + Mag * 0.5f;

        [ShowInInspector]
        public int MaxHp =>
            Mathf.RoundToInt((5 + (Life / 100) * (Con * 3 + Str + Wil) / 3) * 20 * Mathf.Pow(HeightScore, 0.8f));

        [ShowInInspector]
        public int MaxMp => Mathf.RoundToInt(Mana * (Mag + (6 * Mag + 3 * Wil + Lea) / 10) / 5);

        public int MaxCp => 200;

        public int alignment = 0;

        void Start()
        {
            hp = MaxHp;
            mp = MaxMp;
        }

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

        [ShowInInspector]
        public float Height => RawHeight * sizeMultiplier;
        public float HeightScore => Height / 160;
        [ShowInInspector]
        public float Weight => bmi * Mathf.Pow((Height / 100f), 2);


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

        public int ShoeSize
        {
            get { return Mathf.RoundToInt(FootLength * 2 - 10); }
        }

        public enum ResourceType
        {
            Hp,
            Mp,
            Cp,
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

        private void Awake()
        {
            if (preset != null)
            {
                ConsumePreset();
            }
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

        // Update is called once per frame
        void Update()
        {
            
        }
        
        public bool Dead => hp <= 0;
    }

}