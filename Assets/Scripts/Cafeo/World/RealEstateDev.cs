namespace Cafeo.World
{
    /// <summary>
    /// Abstract "real estate developer" that takes an empty map node and populates it.
    /// It does not populate them with agents. They will "move in" later.
    /// </summary>
    public abstract class RealEstateDev
    {
        public class Mall : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                throw new System.NotImplementedException();
            }
        }

        public class Apartments : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                throw new System.NotImplementedException();
            }
        }

        public class SuburbanHouses : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                throw new System.NotImplementedException();
            }
        }

        public class School : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                throw new System.NotImplementedException();
            }
        }

        public class HighSchool : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                throw new System.NotImplementedException();
            }
        }

        public class Road : RealEstateDev
        {
            public override void Furnish(TownOuterNode region)
            {
                throw new System.NotImplementedException();
            }
        }

        public abstract void Furnish(TownOuterNode region);
    }
}