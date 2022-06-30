using System;
using Cafeo.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Cafeo.UI
{
    public class ItemPanel : MonoBehaviour
    {
        private Transform contentPanel;
        public Inventory inventory;
        public Text label;
        public UnityEvent<WorldItem> onClick;
        public bool isShop;

        private void Start()
        {
            contentPanel = GetComponentInChildren<VerticalLayoutGroup>().transform;
            Refresh();
        }

        private void Refresh()
        {
            if (inventory == null)
            {
                label.text = "Inventory Not Found";
                return;
            }
            var manager = WorldUIManager.Instance;
            // destroy all children of contentPanel
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            foreach (var (item, count) in inventory)
            {
                var go = Instantiate(manager.itemPanelTemplate, contentPanel);
                var entry = go.GetComponent<ItemEntry>();
                entry.RegisterItem(item, isShop ? -1 : count);
                entry.onClick.AddListener((it) => onClick.Invoke(it));
            }
        }
    }
}