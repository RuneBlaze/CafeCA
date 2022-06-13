using System;
using Cafeo.Templates;
using UnityEngine;

namespace Cafeo.Data
{
    [Serializable]
    public abstract record GarmentSize
    {
        public enum LetterSize
        {
            Xxxs,
            Xxs,
            Xs,
            S,
            M,
            L,
            Xl,
            Xxl,
            Xxxl,
            Xxxxl,
        }

        public static int LetterSize2ChildHeight(LetterSize size)
        {
            return 100 + 10 * (int)size - (int)LetterSize.Xxs;
        }

        public static float ChildHeight2LetterSize(float height)
        {
            return (height - 100) / 10 + (int)LetterSize.Xxs;
        }

        public static int LetterSize2MenHeight(LetterSize size)
        {
            return 160 + 5 * (int)size - (int)LetterSize.S;
        }
        
        public static float MenHeight2LetterSize(float height)
        {
            return (height - 160) / 5 + (int)LetterSize.S;
        }
        
        public static int LetterSize2WomenHeight(LetterSize size)
        {
            return 155 + 5 * (int)size - (int)LetterSize.S;
        }
        
        public static float WomenHeight2LetterSize(float height)
        {
            return (height - 155) / 5 + (int)LetterSize.S;
        }

        // sizing results are given as float
        // 0 means perfect fit. Anything above 1 and below -1 means it's too small or too big
        // -0.5 - 0 means undersized but wearable
        // 0 - 0.5 means oversized but wearable
        public abstract float CalcFit(AgentSoul soul);
        // public abstract WearableTemplate.GarmentPosition ForPosition();
        public abstract string Localize();
        
        [Serializable]
        public record ShoeSize : GarmentSize
        {
            public readonly int millimeters;

            public ShoeSize(int millimeters)
            {
                this.millimeters = millimeters;
            }
            
            public int EuropeanSize()
            {
                return ((millimeters * 2) - 100) / 10;
            }

            public override float CalcFit(AgentSoul soul)
            {
                var euSize = EuropeanSize();
                if (soul.ShoeSize == euSize) return 0;
                if (soul.ShoeSize == euSize - 1) return -0.5f;
                if (soul.ShoeSize == euSize + 1) return 0.5f;
                return (soul.ShoeSize - euSize) / 2f;
            }

            public override string Localize()
            {
                return millimeters.ToString();
            }
        }

        public record ChildrenSize : GarmentSize
        {
            public readonly LetterSize size;

            public ChildrenSize(LetterSize size)
            {
                this.size = size;
            }

            public override float CalcFit(AgentSoul soul)
            {
                var letter = ChildHeight2LetterSize(soul.Height);
                var idealHeight = LetterSize2ChildHeight(size);
                var heightDiff = letter - idealHeight;
                return heightDiff / 5f;
            }
            public override string Localize()
            {
                return $"{LetterSize2ChildHeight(size)} {size}";
            }
        }
        
        public const float ADULT_TOL = 7.5f;

        public record MenSize : GarmentSize
        {
            public readonly LetterSize size;
            
            public MenSize(LetterSize size)
            {
                this.size = size;
            }

            public override float CalcFit(AgentSoul soul)
            {
                var letter = MenHeight2LetterSize(soul.Height);
                var idealHeight = LetterSize2MenHeight(size);
                var heightDiff = letter - idealHeight;
                return heightDiff / ADULT_TOL;
            }

            public override string Localize()
            {
                return $"{LetterSize2MenHeight(size)} {size}";
            }
        }

        public record WomenSize : GarmentSize
        {
            public readonly LetterSize size;

            public WomenSize(LetterSize size)
            {
                this.size = size;
            }

            public override float CalcFit(AgentSoul soul)
            {
                var letter = WomenHeight2LetterSize(soul.Height);
                var idealHeight = LetterSize2WomenHeight(size);
                var heightDiff = letter - idealHeight;
                return heightDiff / ADULT_TOL;
            }

            public override string Localize()
            {
                return $"{LetterSize2WomenHeight(size)} {size}";
            }
        }
    }
}