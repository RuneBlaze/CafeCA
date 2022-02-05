﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cafeo.Castable
{
    [Serializable]
    public class UsableItem
    {
        public string name;
        public float startUp;
        public float active;
        public float recovery = 0.5f;

        public float cd;
        public bool forceCasting;
        public bool startUpMovable;
        public float baseDamage;

        public bool hitAllies;
        public bool hitEnemies;
        
        public RogueManager Scene => RogueManager.Instance;
        public int targetLayerMask = -1;
        public virtual void Setup(BattleVessel user)
        {
            var hitLayers = new List<string>();
            if (hitAllies)
            {
                hitLayers.Add(user.IsAlly ? "Allies" : "Enemies");
            }

            if (hitEnemies)
            {
                hitLayers.Add(user.IsAlly ? "Enemies" : "Allies");
            }
            
            targetLayerMask = LayerMask.GetMask(hitLayers.ToArray());
            //targetLayerMask = user.IsAlly ? LayerMask.GetMask("Enemies") : LayerMask.GetMask("Allies");
        }

        public virtual void OnUse(BattleVessel user)
        {
            
        }

        public virtual void DuringActive(BattleVessel user, float timer)
        {
            
        }
    }
}