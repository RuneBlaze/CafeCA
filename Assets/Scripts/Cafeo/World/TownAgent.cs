using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.World
{
    public class TownAgent : MonoBehaviour
    {
        public TownNode location;
        
        /// <summary>
        /// Either on the move (travelling) or just down in some rabbit hole
        /// </summary>
        public bool disappeared;
        public int delay;
        public TownAction action;
        // public TownNode goingTo;
        public IEnumerable<(TownNode, int)> Neighbors
        {
            get
            {
                if (location is TownInteriorNode room)
                {
                    foreach (var neighbor in room.Neighbors)
                    {
                        yield return (neighbor, 0);
                    }
                    yield return (room.parent.parent, 0);
                }

                if (location is TownOuterNode outside)
                {
                    foreach (var interiorLocation in outside.interiorLocations)
                    {
                        yield return (interiorLocation.Default, 1);
                    }
                }
            }
        }

        // subscribe to game clock
        public void Subscribe()
        {
            Clock.onTurn.AddListener(OnTurn);
        }
        
        public GameClock Clock => GameClock.Instance;

        public void WalkTo(TownNode target)
        {
            
        }

        public void Act()
        {
            
        }

        /// <summary>
        /// When 5 minutes is passed. Handles system logic.
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
        }

        public void ReachDestination(TownNode node)
        {
            location = node;
        }
        
        /// <summary>
        /// When 5 minutes is passed. Handles user logic.
        /// </summary>
        public void EachTurn()
        {
            
        }
    }
}