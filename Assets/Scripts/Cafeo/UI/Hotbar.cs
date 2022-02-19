using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class Hotbar : MonoBehaviour
    {
        private HotbarItem[] _hotbarItems;
        public RogueManager Scene => RogueManager.Instance;

        [SerializeField] private Text curItemText;
        public void Start()
        {
            _hotbarItems = GetComponentsInChildren<HotbarItem>();
            Refresh();
        }

        private void Refresh()
        {
            var hotbar = Scene.player.hotbar;
            if (hotbar != null)
            {
                var hotbarLength = hotbar.Length;
                for (int i = 0; i < hotbarLength; i++)
                {
                    _hotbarItems[i].SetItem(Scene.player.hotbar[i], Scene.player.hotbarPointer == i);
                }

                if (Scene.player.RetrieveCurItem() != null)
                {
                    curItemText.text = Scene.player.RetrieveCurItem().name;
                }
                
            }
        }
        public void Update()
        {
            Refresh();
        }
    }
}