using System;
using Cafeo.Fashion;
using Cafeo.Templates;

namespace Cafeo.Data
{
    [Serializable]
    public class Wearable
    {
        public WearableSeries series;
        public GarmentSize size;
        public WearableTemplate.GarmentColor color;
        public float constructionQuality;
        public float cutQuality;

        public Wearable(WearableSeries series, GarmentSize size, WearableTemplate.GarmentColor color, float constructionQuality, float cutQuality)
        {
            this.series = series;
            this.size = size;
            this.color = color;
            this.constructionQuality = constructionQuality;
            this.cutQuality = cutQuality;
        }

        public float CalcFit(AgentSoul soul)
        {
            return size.CalcFit(soul);
        }
    }
}