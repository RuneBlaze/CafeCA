using UnityEngine;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class ItemTracker : MonoBehaviour
    {
        private Text[] textDisplays;

        private void Start()
        {
            textDisplays = GetComponentsInChildren<Text>();
            var party = AllyParty.Instance;
            party.onStashChanged.AddListener(Refresh);
            party.onEpChanged.AddListener(_ => Refresh());
            Refresh();
        }

        public void Refresh()
        {
            var party = AllyParty.Instance;
            textDisplays[0].text = $"Coins: {party.Gold}";
            textDisplays[1].text = $"Keys: {party.Keys}";
            textDisplays[2].text = $"Ep: {party.ep / 10}";
        }
    }
}