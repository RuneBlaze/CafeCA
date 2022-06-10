using System.Collections.Generic;
using Cafeo.Fashion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cafeo.Templates
{
    public class FashionShopTemplate : WithDisplayName, ITemplate<FashionBrand>
    {
        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.FashionLine lines;
        
        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.SizeBias sizeBias = WearableTemplate.SizeBias.None;
        
        [BoxGroup("Basic Info", centerLabel: true)] [EnumPaging]
        public List<WearableTemplate.GarmentKind> garmentKinds;

        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.GarmentMaterial composition;

        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.GarmentColor color;
        
        [BoxGroup("Basic Info", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.GarmentColorModifier colorModifier;

        [BoxGroup("Style", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.GarmentStyleAttributes styleAttributes;
        
        [BoxGroup("Style", centerLabel: true)] [EnumToggleButtons]
        public WearableTemplate.GarmentStyleElements styleElements;
        
        [BoxGroup("Historical Attributes", centerLabel: true)]
        [Range(0, 5)]
        public int prestige;

        [BoxGroup("Historical Attributes", centerLabel: true)] [Range(0, 5)]
        public float constructionQuality = 2;
        
        [BoxGroup("Historical Attributes", centerLabel: true)] [Range(0, 2)]
        public float scarcity = 1;

        public string GenName()
        {
            var r = Random.Range(0, 999);
            return $"{displayName} {r}";
        }

        public FashionBrand Generate()
        {
            return FashionBrand.FromTemplate(this);
        }
    }
}