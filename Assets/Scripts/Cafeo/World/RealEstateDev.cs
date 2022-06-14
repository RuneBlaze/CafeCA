using Cafeo.Utils;

namespace Cafeo.World
{
    /// <summary>
    ///     Abstract "real estate developer" that takes an empty map node and populates it.
    ///     It does not populate them with agents. They will "move in" later.
    /// </summary>
    public abstract class RealEstateDev
    {
        public abstract void Furnish(TownOuterNode region);

        public class Mall : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.clayPurple;
            }
        }

        public class Apartments : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.orange;
            }
        }

        public class SuburbanHouses : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.brown;
            }
        }

        public class School : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.pink;
            }
        }

        public class HighSchool : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.red;
            }
        }

        public class Road : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.milkYellow;
            }
        }
    }
}