using System;
using UnityEngine;

namespace Cafeo
{
    
    public class PlayerBrain : GenericBrain
    {
        public BattleVessel vessel;

        private void Start()
        {
            vessel = GetComponent<BattleVessel>();
        }

        public void Update()
        {

        }

        public override BattleVessel Vessel
        {
            get => vessel;
            set => vessel = value;
        }

        public override void DecideAction()
        {
            throw new NotImplementedException();
        }

        public bool PlayerDecideAction()
        {
            var hor = Input.GetAxis("Horizontal");
            var vert = Input.GetAxis("Vertical");
            var dir = new Vector2(hor, vert);
            bool moved = false;
            if (dir.magnitude > 0)
            {
                vessel.Move(dir);
                moved = true;
            }

            bool usedItem = false;
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z))
            {
                usedItem = true;
                vessel.ActivateItem();
            }

            return moved || usedItem;
        }
    }
}