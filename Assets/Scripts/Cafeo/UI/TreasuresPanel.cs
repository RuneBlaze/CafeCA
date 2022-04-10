using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class TreasuresPanel : MonoBehaviour
    {
        private Sprite defaultSprite;
        public RogueManager Scene => RogueManager.Instance;

        private Image[] sprites;
        // private SpriteRenderer sprite;

        public void Awake()
        {
            
        }

        private void Start()
        {
            // sprite = GetComponent<SpriteRenderer>();
            sprites = GetComponentsInChildren<Image>();
            defaultSprite = sprites[0].sprite;
            foreach (var ally in Scene.Allies())
            {
                ally.onGainTreasure.AddListener(treasure => Refresh());
                ally.onLoseTreasure.AddListener(treasure => Refresh());
            }
        }

        public void Refresh()
        {
            var allies = Scene.Allies();
            int i = 1;
            foreach (var ally in allies)
            {
                sprites[i].sprite = ally.HasTreasure ? ally.treasure.Icon : defaultSprite;
                i++;
            }
        }
    }
}