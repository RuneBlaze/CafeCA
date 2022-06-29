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
                region.displayName = "商业街";
            }
        }

        public class Apartments : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.orange;
                region.displayName = "公寓区";
            }
        }

        public class SuburbanHouses : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.brown;
                region.displayName = "居民区";
            }
        }

        public class School : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.pink;
                region.displayName = "小学";
            }
        }

        public class HighSchool : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.red;
                region.displayName = "高中";
            }
        }

        public class Road : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.milkYellow;
                region.displayName = "道路";
            }
        }

        public class Ginza : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                region.color = Palette.milkYellow;
                region.displayName = "银座";
            }
        }
    }
}