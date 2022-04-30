using Cafeo.Castable;
using UnityEngine;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class HotbarItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        public void SetItem(UsableItem item, bool onUse)
        {
            if (item == null)
            {
                image.color = Color.grey;
                return;
            }

            if (item.icon != null)
            {
                // Debug.Log("Sprite already set");
                image.sprite = item.icon;
            }
            else
            {
                image.color = item switch
                {
                    RangedItem => Color.yellow,
                    MeleeItem => Color.blue,
                    TossItem => Color.green,
                    _ => image.color
                };
            }

            if (!onUse)
            {
                image.color = Color.white * 0.5f;
            }
            else
            {
                image.color = Color.white;
            }
        }
    }
}