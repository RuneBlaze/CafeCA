using System.Collections.Generic;
using Cafeo.Data;
using Cafeo.Templates;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Cafeo.Fashion
{
    public class FashionEngine : Singleton<FashionEngine>
    {
        public List<FashionBrand> brands;
        public int currentSeason;

        public UnityEvent onSeasonChange;

        private void Start()
        {
            SimulateFashionSeason();
        }

        protected override void Setup()
        {
            base.Setup();
            onSeasonChange = new UnityEvent();
            brands = new List<FashionBrand>();
            currentSeason = 0;
            GenerateBrands();
        }

        public IEnumerable<WearableSeries> CurrentSeasonCollection()
        {
            foreach (var fashionBrand in brands)
            foreach (var wearableSeries in fashionBrand.collections[currentSeason])
                yield return wearableSeries;
        }

        private void GenerateBrands()
        {
            var finder = TemplateFinder.Instance;
            if (finder.fashionShopTemplates.Count == 0)
            {
                Debug.LogError("No fashion shop templates found");
                return;
            }
            for (var i = 0; i < 30; i++)
            {
                var pool = finder.fashionShopTemplates;
                var e = pool.RandomElement();
                brands.Add(e.Generate());
            }
        }

        private void SimulateFashionSeason()
        {
            foreach (var fashionBrand in brands) fashionBrand.DesignFor(currentSeason);
            onSeasonChange.Invoke();
        }
    }
}