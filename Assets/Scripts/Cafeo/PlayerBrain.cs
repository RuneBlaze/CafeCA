using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Cafeo.Utils;
using UnityEngine;

namespace Cafeo
{
    
    public class PlayerBrain : GenericBrain
    {
        public BattleVessel vessel;
        private List<int> _hotbarKeys;
        private LinkedList<(Vector2, float)> inputHistory; // input axis history for detecting double-tap

        private void Awake()
        {
            inputHistory = new LinkedList<(Vector2, float)>();
        }

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
            if (Scene.inputLocked) return;
            for (int pos = 0; pos < 10; pos++)
            {
                int keyNum = _hotbarKeys[pos];
                if (Input.GetKeyDown(keyNum.ToString())) {
                    if (pos < BattleVessel.HotbarMax)
                    {
                        vessel.TrySetHotboxPointer(pos);
                    }
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
            if (Scene.inputLocked) return false;
            var hor = Input.GetAxisRaw("Horizontal");
            var vert = Input.GetAxisRaw("Vertical");
            var dir = new Vector2(hor, vert);
            var dirSgn = new Vector2(VectorUtils.Sgn(dir.x), VectorUtils.Sgn(dir.y));
            bool doubleTap = false;
            Vector2 dashDir = Vector2.zero;

            // inputHistory[0] is the most recent input
            if ((inputHistory.Count == 0 || inputHistory.First.Value.Item1 != dirSgn))
            {
                // new input
                inputHistory.AddFirst((dirSgn, Time.time));
                if (inputHistory.Count == 4)
                {
                    var oldInput = inputHistory.Last.Previous.Value.Item1;
                    var oldTime = inputHistory.Last.Previous.Value.Item2;
                    var prevPressTime = inputHistory.Last.Value.Item2;
                    var oldSgn = oldInput;
                    // Debug.Log((Time.time - oldTime, oldTime - prevPressTime));
                    if (dir != Vector2.zero && dirSgn == oldSgn 
                                            && Time.time - oldTime < 0.2f)
                    {
                        doubleTap = true;
                        
                        var second = inputHistory.First.Next.Value.Item1;
                        dashDir = dirSgn;
                        // Debug.Log(string.Join(",", inputHistory.Select(it => it.Item1).ToArray()));
                        // Debug.Log(dashDir);
                    }
                }
                
                // remove last
                if (inputHistory.Count > 3)
                {
                    inputHistory.RemoveLast();
                }
            }

            bool moved = false;
            if (dir.magnitude > 0)
            {
                vessel.Move(dir);
                if (doubleTap)
                {
                    vessel.TryDash(dashDir);
                }
                moved = true;
            }

            bool usedItem = false;
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Z))
            {
                usedItem = true;
                vessel.ActivateItem();
            }

            return moved || usedItem;
        }
    }
}