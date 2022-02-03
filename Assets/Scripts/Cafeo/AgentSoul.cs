using System;
using System.Collections;
using System.Collections.Generic;
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

        public string firstName;
        public string lastName;
        public int age = 17;
        public Gender gender = Gender.Male;
        public float heightZ = 0;
        public float bmi = 22.5f;
        public float muscleStrength = 1;
        public float limbSize = 1;
        public float sizeMultiplier = 1;
        public string skinColor = "fair";
        public string hairColor = "black";
        public string eyeColor = "brown";
        public float appearedAgeMultiplier = 1;
        public float genderAppearance = 0.7f;

        public bool hasMaleFeature = false;
        public bool hasFemaleFeature = false;
        public bool hasBreasts = false;
        public float carved = 0.5f;
        public float maleFeatureProminence = 1f;
        public float femaleFeatureProminence = 1f;
        public float appearedAge = 1f;

        public int hp = 12;
        public int mhp = 12;
        public int mp = 6;
        public int mmp = 6;

        public int alignment = 0;

        void Start()
        {

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

        public float Height => RawHeight * sizeMultiplier;
        public float HeightScore => Height / 160;

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

        public void TakeDamage(int k)
        {
            k = Mathf.Clamp(k, 0, hp);
        }

        public void Heal(int k)
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
    }

}