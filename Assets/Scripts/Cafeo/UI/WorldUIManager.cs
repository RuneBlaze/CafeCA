using System;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Cafeo.UI
{
    public class WorldUIManager : Singleton<WorldUIManager>
    {
        public GameObject itemPanelTemplate;

        protected override void Setup()
        {
            base.Setup();
            itemPanelTemplate = Addressables.LoadAssetAsync<GameObject>("Assets/Data/UIPrefabs/ItemPanel.prefab")
                .WaitForCompletion();
        }
    }
}