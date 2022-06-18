using System;
using System.Collections.Generic;
using Cafeo.Utils;
using Cafeo.World;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace Cafeo.UI
{
    public sealed class StageManager : Singleton<StageManager>
    {
        private Dictionary<TownVessel, VesselRepr> reprs;
        private GameObject reprTemplate;
        private TownNode currentScene;
        [SerializeField] private Transform primaryCharacterPanel;
        protected override void Setup()
        {
            base.Setup();
            reprTemplate = Addressables
                .LoadAssetAsync<GameObject>("Assets/Data/UIPrefabs/Repr.prefab")
                .WaitForCompletion();
            reprs = new Dictionary<TownVessel, VesselRepr>();
        }

        private void Start()
        {
            Assert.IsNotNull(primaryCharacterPanel);
            Region.onPlayerMove.AddListener(OnSceneSwitch);
            OnSceneSwitch(Region.player.location);
        }

        private void OnSceneSwitch(TownNode node)
        {
            Debug.Log("Scene Switch");
            // remove everything from the old scene
            TearDownScene();

            currentScene = node;
            // add in everything from the new scene
            SetupScene(node);
        }

        private void SetupScene(TownNode node)
        {
            node.vessels.ForEach(InstantiateRepr);
            node.onVesselEnter.AddListener(OnVesselEnter);
            node.onVesselExit.AddListener(OnVesselLeave);
        }

        private void TearDownScene()
        {
            if (currentScene == null) return;
            reprs.Clear();
            // clear all children of primaryCharacterPanel
            foreach (Transform child in primaryCharacterPanel)
            {
                Destroy(child.gameObject);
            }
            currentScene.onVesselEnter.RemoveListener(OnVesselEnter);
            currentScene.onVesselExit.RemoveListener(OnVesselLeave);
        }

        private void OnVesselLeave(TownVessel vessel)
        {
            var repr = reprs[vessel];
            Destroy(repr.gameObject);
            reprs.Remove(vessel);
        }

        private void OnVesselEnter(TownVessel vessel)
        {
            // Debug.Log("Vessel Enter");
            // Debug.Log("OnVesselEnter" + vessel.soul.DisplayName);
            InstantiateRepr(vessel);
        }

        private void InstantiateRepr(TownVessel vessel)
        {
            var vesselGo = Instantiate(reprTemplate, primaryCharacterPanel);
            var repr = vesselGo.AddComponent<VesselRepr>();
            repr.vessel = vessel;
            reprs[vessel] = repr;
        }

        public TownRegion Region => TownRegion.Instance;
    }
}