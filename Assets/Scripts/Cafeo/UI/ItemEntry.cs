using Cafeo.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class ItemEntry : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text priceLabel;

        public void RegisterItem(WorldItem item)
        {
            icon.sprite = item.icon;
            nameLabel.text = item.displayName;
            priceLabel.text = $"${item.basePrice}";
        }
    }
}