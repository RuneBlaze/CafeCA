using System;
using Cafeo.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class ItemPanel : MonoBehaviour
    {
        private Transform contentPanel;
        public Inventory inventory;
        public UnityEvent<WorldItem> onClick;

        private void Start()
        {
            contentPanel = GetComponentInChildren<VerticalLayoutGroup>().transform;
            Refresh();
        }

        private void Refresh()
        {
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
                entry.RegisterItem(item);
            }
        }
    }
}