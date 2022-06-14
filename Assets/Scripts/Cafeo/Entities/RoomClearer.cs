using Cafeo.MapGen;
using UnityEngine;

namespace Cafeo.Entities
{
    public class RoomClearer : MonoBehaviour
    {
        private bool consumed;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (consumed) return;
            if (col.gameObject.CompareTag("Ally"))
            {
                var go = col.gameObject;
                var vessel = go.GetComponent<BattleVessel>();
                if (vessel.IsLeaderAlly)
                {
                    var generator = RandomMapGenerator.Instance;
                    var room = generator.currentRoom;
                    if (room > 0)
                    {
                        generator.NodeById(room).counter = 0;
                        consumed = true;
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}