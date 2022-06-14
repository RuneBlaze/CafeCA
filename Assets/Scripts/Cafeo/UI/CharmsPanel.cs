using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class CharmsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject charmPrefab;

        private void Start()
        {
            Assert.IsNotNull(charmPrefab);
            var party = AllyParty.Instance;
            party.onGainCharm.AddListener(charm => Resync());
        }

        private void Resync()
        {
            var party = AllyParty.Instance;
            foreach (Transform child in transform) Destroy(child.gameObject);
            foreach (var charm in party.charms)
            {
                var go = Instantiate(charmPrefab, transform);
                go.GetComponent<Image>().sprite = charm.Icon;
            }
        }
    }
}