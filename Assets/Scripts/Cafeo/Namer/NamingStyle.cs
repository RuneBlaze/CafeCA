using Cafeo.Templates;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo.Namer
{
    public abstract class NamingStyle
    {
        public static readonly string[] defaultLines =
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };

        public abstract string NameSome(WearableTemplate.GarmentKind garmentKind);

        public class NumericsNamingStyle : NamingStyle
        {
            public override string NameSome(WearableTemplate.GarmentKind garmentKind)
            {
                var num = (int)garmentKind * 100 + Random.Range(0, 99);
                return $"{num}";
            }
        }

        public class AbstractWordNamingStyle : NamingStyle
        {
            public override string NameSome(WearableTemplate.GarmentKind garmentKind)
            {
                return defaultLines.RandomElement();
            }
        }
    }
}