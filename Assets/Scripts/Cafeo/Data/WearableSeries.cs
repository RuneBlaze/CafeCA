using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Fashion;
using Cafeo.Templates;

namespace Cafeo.Data
{
    [Serializable]
    public class WearableSeries
    {
        public List<WearableTemplate.GarmentColor> colorRange;

        public string displayName;
        public WearableTemplate.FashionLine line;
        public WearableTemplate.GarmentKind kind;
        public FashionBrand brand;

        public WearableTemplate.GarmentMaterial composition;

        // public WearableTemplate.GarmentColor color;
        public WearableTemplate.GarmentColorModifier colorModifier;
        public WearableTemplate.GarmentStyleAttributes styleAttributes;
        public WearableTemplate.GarmentStyleElements styleElements;

        public float constructionQuality;
        public float cutQuality;

        public float aestheticsQuality;

        // public Wearable wearable;
        public List<GarmentSize> sizeRange;

        public WearableSeries(List<GarmentSize> sizeRange, List<WearableTemplate.GarmentColor> colorRange,
            string displayName, WearableTemplate.FashionLine line, WearableTemplate.GarmentKind kind,
            FashionBrand brand, WearableTemplate.GarmentMaterial composition,
            WearableTemplate.GarmentColorModifier colorModifier,
            WearableTemplate.GarmentStyleAttributes styleAttributes,
            WearableTemplate.GarmentStyleElements styleElements, float constructionQuality, float cutQuality,
            float aestheticsQuality)
        {
            this.sizeRange = sizeRange;
            this.colorRange = colorRange;
            this.displayName = displayName;
            this.line = line;
            this.kind = kind;
            this.brand = brand;
            this.composition = composition;
            this.colorModifier = colorModifier;
            this.styleAttributes = styleAttributes;
            this.styleElements = styleElements;
            this.constructionQuality = constructionQuality;
            this.cutQuality = cutQuality;
            this.aestheticsQuality = aestheticsQuality;
        }

        public WearableTemplate.GarmentPosition Position => WearableTemplate.GarmentKind2Position(kind);

        public void AddTo(List<Wearable> collection)
        {
            collection.AddRange(from garmentSize in sizeRange from garmentColor in colorRange select new Wearable(this, garmentSize, garmentColor, constructionQuality, cutQuality));
        }

        public IEnumerable<Wearable> Contents()
        {
            return from garmentSize in sizeRange
                from garmentColor in colorRange
                select new Wearable(this, garmentSize, garmentColor, constructionQuality, cutQuality);
        }
    }
}