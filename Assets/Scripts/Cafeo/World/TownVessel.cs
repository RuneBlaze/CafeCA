using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.World
{
    public class TownVessel : MonoBehaviour
    {
        public TownOuterNode outerNode;
        public TownNode location;

        /// <summary>
        ///     Either on the move (travelling) or just down in some rabbit hole
        /// </summary>
        public bool disappeared;

        public int delay;

        public TownAction action;
        public AgentSoul soul;
        public TownBrain brain;

        public bool IsPlayer => Region.player == this;
        public TownRegion Region => TownRegion.Instance;
        
        public bool Idle => action == null;

        private void Start()
        {
            brain = GetComponent<TownBrain>();
        }

        // public TownNode goingTo;
        public IEnumerable<(TownNode, int)> Neighbors
        {
            get
            {
                if (location is TownInteriorNode room)
                {
                    foreach (var neighbor in room.Neighbors) yield return (neighbor, 0);
                    yield return (room.parent.parent, 0);
                }

                if (location is TownOuterNode outside)
                    foreach (var interiorLocation in outside.interiorLocations)
                        yield return (interiorLocation.Default, 1);
            }
        }

        public GameClock Clock => GameClock.Instance;

        // subscribe to game clock
        public void Subscribe()
        {
            Clock.onTurn.AddListener(OnTurn);
        }

        public void WalkTo(TownNode target)
        {
        }

        /// <summary>
        /// When not blocked, can actually think
        /// </summary>
        public void Act()
        {
            brain.DecideAction();
        }

        /// <summary>
        ///     When 5 minutes is passed. Handles system logic.
        /// </summary>
        public void OnTurn()
        {
            if (action != null)
            {
                action.duration--;
                if (action.duration <= 0)
                {
                    action.OnEnd();
                    OnActionFinished(action);
                }
            }
            else
            {
                Act();
            }
            EachTurn();
        }

        public virtual void OnActionStarted(TownAction act)
        {
            disappeared = act.Disappearing;
        }

        public virtual void OnActionFinished(TownAction act)
        {
            disappeared = false;
            action = null;
        }

        public void ReachDestination(TownNode node)
        {
            // location = node;
            location.Transfer(this, node);
            if (IsPlayer)
            {
                Region.onPlayerMove.Invoke(node);
            }
        }

        /// <summary>
        ///     When 5 minutes is passed. Handles user logic.
        /// </summary>
        public void EachTurn()
        {
        }
    }
}