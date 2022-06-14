using System;
using Cafeo.Templates;

namespace Cafeo.Data
{
    [Serializable]
    public class Wearable
    {
        public WearableSeries series;
        public WearableTemplate.GarmentColor color;
        public float constructionQuality;
        public float cutQuality;
        public GarmentSize size;

        public Wearable(WearableSeries series, GarmentSize size, WearableTemplate.GarmentColor color,
            float constructionQuality, float cutQuality)
        {
            this.series = series;
            this.size = size;
            this.color = color;
            this.constructionQuality = constructionQuality;
            this.cutQuality = cutQuality;
        }

        public int Layer => WearableTemplate.GarmentKind2Layering(series.kind);
        public WearableTemplate.GarmentPosition Position => WearableTemplate.GarmentKind2Position(series.kind);

        public float CalcFit(AgentSoul soul)
        {
            return size.CalcFit(soul);
        }
    }
}