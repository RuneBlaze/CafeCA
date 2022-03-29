using Cafeo.Templates;

namespace Cafeo.Data
{
    public abstract record GarmentSize
    {
        public enum LetterSize
        {
            XXXS,
            XXS,
            XS,
            S,
            M,
            L,
            XL,
            XXL,
            XXXL,
            XXXXL,
        }
        
        // sizing results are given as float
        // 0 means perfect fit. Anything above 1 and below -1 means it's too small or too big
        // -0.5 - 0 means undersized but wearable
        // 0.5 - 1 means oversized but wearable
        public abstract float CalcFit(AgentSoul soul);
        public abstract WearableTemplate.GarmentPosition ForPosition();
        public abstract string Localize();
        
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

            public override WearableTemplate.GarmentPosition ForPosition()
            {
                return WearableTemplate.GarmentPosition.Foot;
            }

            public override string Localize()
            {
                return millimeters.ToString();
            }
        }
    }
}