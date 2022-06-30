using System;
using Cafeo.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Cafeo.UI
{
    public class ItemEntry : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text priceLabel;
        [SerializeField] private WorldItem item;
        private TooltipTrigger tooltipTrigger;
        public UnityEvent<WorldItem> onClick;

        private void Awake()
        {
            onClick = new UnityEvent<WorldItem>();
        }

        private void Start()
        {
            tooltipTrigger = GetComponent<TooltipTrigger>();
        }

        private void Update()
        {
            if (tooltipTrigger.text.Length <= 0)
            {
                if (item != null)
                {
                    tooltipTrigger.text = item.tooltip;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count">Set to -1 to show the price instead.</param>
        public void RegisterItem(WorldItem item, int count)
        {
            icon.sprite = item.icon;
            nameLabel.text = item.displayName;
            this.item = item;
            if (count < 0)
            {
                priceLabel.text = $"${item.basePrice}";
            }
            else
            {
                priceLabel.text = $"x{count}";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (item != null)
            {
                onClick.Invoke(item);
            }
        }
    }
}