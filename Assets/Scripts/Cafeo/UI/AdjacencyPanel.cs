using System;
using Cafeo.World;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Cafeo.UI
{
    /// <summary>
    /// UI Controller for the panel that displays adjacent locations.
    /// </summary>
    public class AdjacencyPanel : MonoBehaviour
    {
        [SerializeField] private Text currentLabel;
        [SerializeField] private Text northLabel;
        [SerializeField] private Text southLabel;
        [SerializeField] private Text eastLabel;
        [SerializeField] private Text westLabel;
        [SerializeField] private Transform interiorPanel;

        private void Awake()
        {
            Assert.IsNotNull(currentLabel);
            Assert.IsNotNull(northLabel);
            Assert.IsNotNull(southLabel);
            Assert.IsNotNull(eastLabel);
            Assert.IsNotNull(westLabel);
            Assert.IsNotNull(interiorPanel);
        }

        public TownRegion Region => TownRegion.Instance;

        private void Start()
        {
            Region.initFinished.AddListener(Sync);
            Region.onPlayerMove.AddListener(_ => Sync());
        }

        private void Sync()
        {
            var playerLoc = Region.player.location;
            currentLabel.text = playerLoc.displayName;
            switch (playerLoc)
            {
                case TownOuterNode outerNode:
                    foreach (var (townOuterNode, x, y) in Region.OuterNeighborsWithRelCoord(outerNode))
                    {
                        var displayName = townOuterNode.displayName;
                        switch ((x, y))
                        {
                            case (0, 1):
                                northLabel.text = displayName;
                                break;
                            case (0, -1):
                                southLabel.text = displayName;
                                break;
                            case (1, 0):
                                eastLabel.text = displayName;
                                break;
                            case (-1, 0):
                                westLabel.text = displayName;
                                break;
                        }
                    }
                    break;
                default:
                    northLabel.text = "";
                    southLabel.text = "";
                    eastLabel.text = "";
                    westLabel.text = "";
                    break;
            }
        }
    }
}