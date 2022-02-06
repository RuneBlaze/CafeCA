using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Cafeo
{
    
    public class PlayerBrain : GenericBrain
    {
        public BattleVessel vessel;
        private List<int> _hotbarKeys;

        public override void Start()
        {
            base.Start();
            vessel = GetComponent<BattleVessel>();
            _hotbarKeys = new List<int>() {1,2,3,4,5,6,7,8,9,0};
            SetupLeaderFollow();
        }

        private void SetupLeaderFollow()
        {
            BehaviorTree.SetVariableValue("Leader", Scene.leaderAlly);
            BehaviorTree.SetVariableValue("Followers", 
                Scene.otherAllyVessels.Select(it => it.gameObject).ToList());
            BehaviorTree.EnableBehavior();
        }

        public void Update()
        {
            for (int pos = 0; pos < 10; pos++)
            {
                int keyNum = _hotbarKeys[pos];
                if (Input.GetKeyDown(keyNum.ToString())) {
                    vessel.TrySetHotboxPointer(pos);
                }
            }
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