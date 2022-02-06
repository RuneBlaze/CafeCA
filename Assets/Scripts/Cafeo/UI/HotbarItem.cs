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

            image.color = item switch
            {
                RangedItem => Color.yellow,
                MeleeItem => Color.blue,
                TossItem => Color.green,
                _ => image.color
            };

            if (!onUse)
            {
                image.color /= 2;
            }
        }
    }
}