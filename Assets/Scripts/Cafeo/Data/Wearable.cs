using System;
using Cafeo.Templates;

namespace Cafeo.Data
{
    [Serializable]
    public class Wearable : WorldItem
    {
        public WearableSeries series;
        public WearableTemplate.GarmentColor color;
        public float constructionQuality;
        public float cutQuality;
        public WearableTemplate.FashionLine line;
        public WearableTemplate.GarmentKind kind;
        public GarmentSize size;

        public Wearable(WearableSeries series, GarmentSize size, WearableTemplate.GarmentColor color,
            float constructionQuality, float cutQuality)
        {
            displayName = $"[{size.Localize()}] {series.displayName}";
            this.series = series;
            this.size = size;
            this.color = color;
            this.line = series.line;
            this.kind = series.kind;
            tooltip = GenerateTooltip();
            this.constructionQuality = constructionQuality;
            this.cutQuality = cutQuality;
        }

        public string GenerateTooltip()
        {
            return $"{displayName} {kind}";
        }

        public int Layer => WearableTemplate.GarmentKind2Layering(series.kind);
        public WearableTemplate.GarmentPosition Position => WearableTemplate.GarmentKind2Position(series.kind);

        public float CalcFit(AgentSoul soul)
        {
            return size.CalcFit(soul);
        }
    }
}