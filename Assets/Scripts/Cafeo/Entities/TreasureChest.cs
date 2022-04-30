using Cafeo.Data;
using UnityEngine;

namespace Cafeo.Entities
{
    public class TreasureChest : MonoBehaviour
    {
        public DropInventory inventory;
        private bool consumed;
        private void OnCollisionEnter2D(Collision2D col)
        {
            var scene = RogueManager.Instance;
            if (consumed) return;
            if (col.gameObject.CompareTag("Ally"))
            {
                var go = col.gameObject;
                consumed = true;
                var position = transform.position;
                var popDir = position - go.transform.position;
                scene.PopDropInventory(position, inventory, popDir.normalized);
                Destroy(gameObject);
            }
        }
    }
}