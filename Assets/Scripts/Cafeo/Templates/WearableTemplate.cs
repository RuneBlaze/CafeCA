using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.ObjectDrawers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class WearableTemplate : WithIcon
    {
        [BoxGroup("Basic Info", centerLabel: true)]
        [HideLabel, TextArea(4, 14)]
        public string description;

        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public FashionLine lines;
        
        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public SizeBias sizeBias = SizeBias.None;
        
        [BoxGroup("Basic Info", centerLabel: true)] [EnumPaging]
        public GarmentKind garmentKind = GarmentKind.Shirt;

        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public GarmentMaterial composition;

        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public GarmentColor color;
        
        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public GarmentColorModifier colorModifier;

        [BoxGroup("Style", centerLabel: true)] [EnumToggleButtons]
        public GarmentStyleAttributes styleAttributes;
        
        [BoxGroup("Style", centerLabel: true)] [EnumToggleButtons]
        public GarmentStyleElements styleElements;

        // [BoxGroup("Historical Attributes", centerLabel: true)]
        // public int yearDesigned;
        //
        // [BoxGroup("Historical Attributes", centerLabel: true)] [EnumPaging]
        // public FashionSeason seasonDesigned;

        // [BoxGroup("Historical Attributes", centerLabel: true)]
        // [Range(0, 5)]
        // public int prestige;
        //
        // [BoxGroup("Historical Attributes", centerLabel: true)] [Range(0, 5)]
        // public float constructionQuality = 2;

        [System.Flags]
        public enum FashionLine
        {
            Unisex = 1 << 1,
            Men = 1 << 2,
            Women = 1 << 3,
            Children = 1 << 4,
            Boys = 1 << 5,
            Girls = 1 << 6,
        }

        public enum SizeBias
        {
            None,
            ForSmall,
            ForLarge,
        }

        public enum GarmentKind
        {
            Weapon,
            Tee,
            Shirt,
            Cardigan,
            Coat,
            Pant,
            Blouse,
            Dress,
            Skirt,
            Socks,
            Sneakers,
            Boots,
            Slippers,
            Hat,
            Jewelry,
            UndergarmentBottom,
            UndergarmentTop,
        }

        [Flags]
        public enum GarmentPosition
        {
            Hand = 1 << 1,
            Foot = 1 << 2,
            Head = 1 << 3,
            UpperBody = 1 << 4,
            LowerBody = 1 << 5,
            Back = 1 << 6,
            Other = 1 << 7,
        }

        public static GarmentPosition GarmentKind2Position(GarmentKind kind)
        {
            return kind switch
            {
                GarmentKind.Weapon => GarmentPosition.Hand,
                GarmentKind.Tee => GarmentPosition.UpperBody,
                GarmentKind.Shirt => GarmentPosition.UpperBody,
                GarmentKind.Cardigan => GarmentPosition.UpperBody,
                GarmentKind.Coat => GarmentPosition.UpperBody,
                GarmentKind.Pant => GarmentPosition.LowerBody,
                GarmentKind.Blouse => GarmentPosition.UpperBody,
                GarmentKind.Dress => GarmentPosition.UpperBody | GarmentPosition.LowerBody,
                GarmentKind.Skirt => GarmentPosition.LowerBody,
                GarmentKind.Socks => GarmentPosition.UpperBody,
                GarmentKind.Sneakers => GarmentPosition.Foot,
                GarmentKind.Boots => GarmentPosition.Foot,
                GarmentKind.Slippers => GarmentPosition.Foot,
                GarmentKind.Hat => GarmentPosition.Head,
                GarmentKind.Jewelry => GarmentPosition.Other,
                GarmentKind.UndergarmentBottom => GarmentPosition.LowerBody,
                GarmentKind.UndergarmentTop => GarmentPosition.UpperBody,
                _ => GarmentPosition.Other,
            };
        }

        [System.Flags]
        public enum GarmentMaterial
        {
            Leather = 1 << 1,
            Cotton = 1 << 2,
            Silk = 1 << 3,
            Metal = 1 << 4,
            Wood = 1 << 5,
            Plastic = 1 << 6,
            Glass = 1 << 7,
            Paper = 1 << 8,
            Rubber = 1 << 9,
            Stone = 1 << 10,
            Polyester = 1 << 11,
        }

        public enum GarmentColor
        {
            Raw,
            White,
            Black,
            Red,
            Blue,
            Green,
            Yellow,
            Purple,
            Orange,
            Brown,
            Gray,
        }

        public enum GarmentColorModifier
        {
            None,
            Pastel,
            Light,
            Dark,
            Neutral,
            Patterned,
            DarkBlend,
            LightBlend,
        }

        public enum FashionSeason
        {
            NoSpecific,
            SpringSummer,
            AutumnWinter,
            Cruise,
        }

        [Serializable]
        public class GarmentStyleAttributes
        {
            [Range(-2, 2)]
            public float seriousness;
            
            [Range(-2, 2)]
            public float structure;
            
            [Range(-2, 2)]
            public float heaviness;
        }

        [Flags]
        public enum GarmentStyleElements
        {
            None = 0,
            Romantic = 1 << 1,
        }
    }
}