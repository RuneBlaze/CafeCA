using UnityEngine;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class Hotbar : MonoBehaviour
    {
        [SerializeField] private Text curItemText;
        private HotbarItem[] _hotbarItems;
        public RogueManager Scene => RogueManager.Instance;

        public void Start()
        {
            _hotbarItems = GetComponentsInChildren<HotbarItem>();
            Refresh();
        }

        public void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            var hotbar = Scene.player.hotbar;
            if (hotbar != null)
            {
                var hotbarLength = hotbar.Length;
                for (var i = 0; i < hotbarLength; i++)
                    _hotbarItems[i].SetItem(Scene.player.hotbar[i], Scene.player.hotbarPointer == i);

                if (Scene.player.RetrieveCurItem() != null) curItemText.text = Scene.player.RetrieveCurItem().name;
            }
        }
    }
}