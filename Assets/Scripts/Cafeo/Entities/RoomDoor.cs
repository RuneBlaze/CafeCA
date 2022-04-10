using Cafeo.MapGen;
using UnityEngine;

namespace Cafeo.Entities
{
    public class RoomDoor : MonoBehaviour
    {
        public MapNode lhs;
        public MapNode rhs;

        public bool horizontal;

        public void LateInit(MapNode lhs, MapNode rhs, bool horizontal)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.horizontal = horizontal;
            lhs.onStateChanged.AddListener(Refresh);
            rhs.onStateChanged.AddListener(Refresh);
            if (!horizontal)
            {
                transform.eulerAngles = Vector3.forward * 90;
            }
            Refresh();
        }

        private void Refresh()
        {
            if (lhs.state == MapNode.State.Active || rhs.state == MapNode.State.Active)
            {
                gameObject.SetActive(true);
            }
            else if (lhs.state == MapNode.State.Unexplored && rhs.state == MapNode.State.Unexplored)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}