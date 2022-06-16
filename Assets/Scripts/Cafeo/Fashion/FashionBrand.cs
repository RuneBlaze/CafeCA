using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Data;
using Cafeo.Namer;
using Cafeo.Templates;
using Cafeo.Utils;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cafeo.Fashion
{
    [Serializable]
    public class FashionBrand
    {
        public static readonly GarmentSize.ShoeSize[] menShoeSizes =
            Enumerable.Range(0, 10).Select(i => new GarmentSize.ShoeSize(245 + 5 * i)).ToArray();

        public static readonly GarmentSize.ShoeSize[] largeMenShoeSizes =
            Enumerable.Range(0, 10).Select(i => new GarmentSize.ShoeSize(265 + 5 * i)).ToArray();

        public static readonly GarmentSize.ShoeSize[] womenShoeSizes =
            Enumerable.Range(0, 10).Select(i => new GarmentSize.ShoeSize(225 + 5 * i)).ToArray();

        public static readonly GarmentSize.ShoeSize[] largeWomenShoeSizes =
            Enumerable.Range(0, 10).Select(i => new GarmentSize.ShoeSize(245 + 5 * i)).ToArray();

        public static readonly GarmentSize.ShoeSize[] boyShoeSizes =
            Enumerable.Range(0, 14).Select(i => new GarmentSize.ShoeSize(180 + 5 * i)).ToArray();

        public static readonly GarmentSize.ShoeSize[] girlShoeSizes =
            Enumerable.Range(0, 12).Select(i => new GarmentSize.ShoeSize(180 + 5 * i)).ToArray();

        public static readonly GarmentSize.LetterSize[] normalLetters =
        {
            GarmentSize.LetterSize.Xs, GarmentSize.LetterSize.S, GarmentSize.LetterSize.M, GarmentSize.LetterSize.L,
            GarmentSize.LetterSize.Xl, GarmentSize.LetterSize.Xxl
        };

        public static readonly GarmentSize.LetterSize[] largeLetters =
        {
            GarmentSize.LetterSize.L,
            GarmentSize.LetterSize.Xl, GarmentSize.LetterSize.Xxl, GarmentSize.LetterSize.Xxxl,
            GarmentSize.LetterSize.Xxxxl
        };

        public static readonly GarmentSize.LetterSize[] smallLetters =
        {
            GarmentSize.LetterSize.Xxxs, GarmentSize.LetterSize.Xxs, GarmentSize.LetterSize.Xs,
            GarmentSize.LetterSize.S
        };

        public static readonly GarmentSize.MenSize[] menGarmentSizes =
            normalLetters.Select(it => new GarmentSize.MenSize(it)).ToArray();

        public static readonly GarmentSize.WomenSize[] womenGarmentSizes =
            normalLetters.Select(it => new GarmentSize.WomenSize(it)).ToArray();

        public static readonly GarmentSize.ChildrenSize[] childrenGarmentSizes =
            largeLetters.Select(it => new GarmentSize.ChildrenSize(it)).ToArray();

        public static readonly GarmentSize.MenSize[] largeMenGarmentSizes =
            largeLetters.Select(it => new GarmentSize.MenSize(it)).ToArray();

        public static readonly GarmentSize.WomenSize[] largeWomenGarmentSizes =
            largeLetters.Select(it => new GarmentSize.WomenSize(it)).ToArray();

        public string displayName;
        public WearableTemplate.FashionLine lines;
        public WearableTemplate.SizeBias sizeBias = WearableTemplate.SizeBias.None;
        public WearableTemplate.GarmentMaterial composition;
        public WearableTemplate.GarmentColor color;
        public WearableTemplate.GarmentColorModifier colorModifier;
        public WearableTemplate.GarmentStyleAttributes styleAttributes;
        public WearableTemplate.GarmentStyleElements styleElements;
        public int prestige;
        public float constructionQuality;
        public float scarcity;

        public Dictionary<int, List<WearableSeries>> collections;
        public HashSet<WearableTemplate.GarmentKind> garmentKinds;
        public NamingStyle namingStyle;

        public FashionBrand(string displayName, WearableTemplate.FashionLine lines,
            List<WearableTemplate.GarmentKind> garmentKinds, WearableTemplate.SizeBias sizeBias,
            WearableTemplate.GarmentMaterial composition, WearableTemplate.GarmentColor color,
            WearableTemplate.GarmentColorModifier colorModifier,
            WearableTemplate.GarmentStyleAttributes styleAttributes,
            WearableTemplate.GarmentStyleElements styleElements, NamingStyle namingStyle, int prestige,
            float constructionQuality,
            float scarcity)
        {
            this.displayName = displayName;
            this.lines = lines;
            this.garmentKinds = new HashSet<WearableTemplate.GarmentKind>();
            this.garmentKinds.AddRange(garmentKinds);
            this.sizeBias = sizeBias;
            this.composition = composition;
            this.color = color;
            this.colorModifier = colorModifier;
            this.styleAttributes = styleAttributes;
            this.styleElements = styleElements;
            this.prestige = prestige;
            this.constructionQuality = constructionQuality;
            this.scarcity = scarcity;
            this.namingStyle = namingStyle;
            collections = new Dictionary<int, List<WearableSeries>>();
        }

        public static FashionBrand FromTemplate(FashionShopTemplate template)
        {
            return new FashionBrand(template.GenName(), template.lines, template.garmentKinds, template.sizeBias,
                template.composition,
                template.color, template.colorModifier, template.styleAttributes, template.styleElements,
                template.GenNamingStyle(),
                template.prestige, template.constructionQuality, template.scarcity);
        }

        public float ScoreWearableTemplate(WearableTemplate template)
        {
            if ((int)(template.EffectiveLine & lines) == 0) return -1;
            if (!garmentKinds.Contains(template.garmentKind)) return -1;
            if (template.sizeBias != sizeBias) return -1;
            return 1;
        }

        public static GarmentSize[] SizingFromLineKindBias(WearableTemplate.FashionLine line,
            WearableTemplate.GarmentKind kind, WearableTemplate.SizeBias bias)
        {
            switch (line)
            {
                case WearableTemplate.FashionLine.Unisex:
                case WearableTemplate.FashionLine.Men:
                    return bias switch
                    {
                        WearableTemplate.SizeBias.None => menGarmentSizes,
                        WearableTemplate.SizeBias.ForSmall => menGarmentSizes,
                        WearableTemplate.SizeBias.ForLarge => largeMenGarmentSizes,
                        _ => throw new ArgumentOutOfRangeException(nameof(bias), bias, null)
                    };
                case WearableTemplate.FashionLine.Women:
                    return bias switch
                    {
                        WearableTemplate.SizeBias.None => womenGarmentSizes,
                        WearableTemplate.SizeBias.ForSmall => womenGarmentSizes,
                        WearableTemplate.SizeBias.ForLarge => largeWomenGarmentSizes,
                        _ => throw new ArgumentOutOfRangeException(nameof(bias), bias, null)
                    };
                case WearableTemplate.FashionLine.Children:
                case WearableTemplate.FashionLine.Boys:
                case WearableTemplate.FashionLine.Girls:
                    if (WearableTemplate.IsShoe(kind))
                        return (line, bias) switch
                        {
                            (WearableTemplate.FashionLine.Boys, _) => boyShoeSizes,
                            (WearableTemplate.FashionLine.Girls, _) => boyShoeSizes,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    return bias switch
                    {
                        _ => childrenGarmentSizes
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(line), line, null);
            }
        }

        public WearableSeries DesignSingleItem(WearableTemplate template)
        {
            var availableLines = new List<WearableTemplate.FashionLine>();
            Enum.GetValues(typeof(WearableTemplate.FashionLine)).Cast<WearableTemplate.FashionLine>().ToList().ForEach(
                it =>
                {
                    if ((int)(it & lines & template.EffectiveLine) != 0) availableLines.Add(it);
                });
            var l = availableLines.RandomElement();
            var bias = template.sizeBias;
            var sizing = SizingFromLineKindBias(l, template.garmentKind, bias);
            var colors = new List<WearableTemplate.GarmentColor>();
            if (template.color == WearableTemplate.GarmentColor.Raw)
                colors.AddRange(Enum.GetValues(typeof(WearableTemplate.GarmentColor))
                    .Cast<WearableTemplate.GarmentColor>());
            else
                colors.Add(template.color);

            var rawName = namingStyle.NameSome(template.garmentKind);

            return new WearableSeries(sizing.ToList(), colors, $"{displayName} {rawName}", l, template.garmentKind,
                this,
                template.composition, template.colorModifier, template.styleAttributes, styleElements,
                constructionQuality + Random.Range(-0.5f, 0.5f),
                prestige + Random.Range(-0.5f, 0.5f),
                prestige + Random.Range(-0.5f, 0.5f));
        }

        public void DesignFor(int fashionSeason)
        {
            if (!collections.ContainsKey(fashionSeason)) collections.Add(fashionSeason, new List<WearableSeries>());

            var thisSeason = collections[fashionSeason];
            var items = Mathf.RoundToInt(scarcity * 40);
            var templateFinder = TemplateFinder.Instance;
            var feasible = templateFinder.wearableTemplates.Where(it => ScoreWearableTemplate(it) >= 0).ToList();
            feasible.Sort((a, b) =>
                ScoreWearableTemplate(a).CompareTo(ScoreWearableTemplate(b)));
            if (feasible.Count == 0)
            {
                // Debug.Log("I have nothing to design for season " + fashionSeason);
                return;
            }

            for (var i = 0; i < items; i++)
            {
                var template = feasible.RandomElement();
                var series = DesignSingleItem(template);
                thisSeason.Add(series);
                // Debug.Log("I designed one " + series.displayName + " for season " + fashionSeason);
            }
        }
    }
}