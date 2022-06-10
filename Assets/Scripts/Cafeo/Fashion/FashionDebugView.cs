using System;
using System.Collections.Generic;
using Cafeo.Data;
using UnityEngine;

namespace Cafeo.Fashion
{
    public class FashionDebugView : MonoBehaviour
    {
        public List<WearableSeries> fashionHistory;
        public void Start()
        {
            fashionHistory = new List<WearableSeries>();
            FashionEngine.Instance.onSeasonChange.AddListener(OnSeasonChange);
        }
        
        public void OnSeasonChange()
        {
            Debug.Log("OnSeasonChange");
            foreach (var wearableSeries in FashionEngine.Instance.CurrentSeasonCollection())
            {
                fashionHistory.Add(wearableSeries);
            }
        }
    }
}