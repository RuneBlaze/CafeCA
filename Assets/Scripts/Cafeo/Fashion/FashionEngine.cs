using System;
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

        protected override void Setup()
        {
            base.Setup();
            onSeasonChange = new UnityEvent();
            brands = new List<FashionBrand>();
            currentSeason = 0;
            GenerateBrands();
        }

        private void Start()
        {
            SimulateFashionSeason();
        }

        public IEnumerable<WearableSeries> CurrentSeasonCollection()
        {
            foreach (var fashionBrand in brands)
            {
                foreach (var wearableSeries in fashionBrand.collections[currentSeason])
                {
                    yield return wearableSeries;
                }
            }
        }

        private void GenerateBrands()
        {
            var finder = TemplateFinder.Instance;
            for (int i = 0; i < 30; i++)
            {
                var e = finder.fashionShopTemplates.RandomElement();
                brands.Add(e.Generate());
            }
        }

        private void SimulateFashionSeason()
        {
            foreach (var fashionBrand in brands)
            {
                fashionBrand.DesignFor(currentSeason);
            }
            onSeasonChange.Invoke();
            // Debug.Log("Season " + currentSeason);
        }
    }
}