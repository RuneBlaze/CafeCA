using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class WearableTemplate : WithIcon
    {
        [BoxGroup("Basic Info", centerLabel: true)]
        [HideLabel, TextArea(4, 14)]
        public string description;

        [BoxGroup("Basic Info", centerLabel: true)]
        public List<FashionLine> lines;
        
        [BoxGroup("Basic Info", centerLabel: true)]
        public SizeBias sizeBias = SizeBias.None;
        
        [BoxGroup("Basic Info", centerLabel: true)]
        public GarmentKind garmentKind = GarmentKind.Top;

        [BoxGroup("Basic Info", centerLabel: true)]
        public List<GarmentMaterial> composition;

        public enum FashionLine
        {
            Unisex,
            Men,
            Women,
            Children,
            Boys,
            Girls,
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
            Top,
            Bottom,
            Undergarment,
            Accessory,
            Shoe,
        }

        public enum GarmentMaterial
        {
            Leather,
            Cloth,
            Metal,
            Wood,
            Plastic,
            Glass,
            Paper,
            Rubber,
            Stone,
            Polyester,
            Nylon,
        }
    }
}