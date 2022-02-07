using System;
using System.Collections;
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

        public IEnumerator coroutineOnStart;
        
        public RogueManager Scene => RogueManager.Instance;
        public int targetLayerMask = -1;
        public string targetTag;
        private bool coroutineDone;
        private Coroutine activeCoroutine;
        public virtual void Setup(BattleVessel user)
        {
            var hitLayers = new List<string>();
            if (hitAllies)
            {
                hitLayers.Add(user.IsAlly ? "Allies" : "Enemies");
                targetTag = user.IsAlly ? "Ally" : "Enemy";
            }

            if (hitEnemies)
            {
                hitLayers.Add(user.IsAlly ? "Enemies" : "Allies");
                targetTag = user.IsAlly ? "Enemy" : "Ally";
            }

            targetLayerMask = LayerMask.GetMask(hitLayers.ToArray());
            if (coroutineOnStart != null)
            {
                activeCoroutine = null;
                coroutineDone = false;
                active = 1231231234;
            }
        }

        private IEnumerator WrappedCoroutine(BattleVessel user)
        {
            yield return coroutineOnStart;
            coroutineDone = true;
            user.ApplyActiveItemStun();
            yield return null;
        }

        public virtual void OnUse(BattleVessel user)
        {
            if (coroutineOnStart != null)
            {
                coroutineDone = false;
                activeCoroutine = user.StartCoroutine(WrappedCoroutine(user));
            }
        }

        public virtual void OnCounter(BattleVessel user)
        {
            if (activeCoroutine != null)
            {
                user.StopCoroutine(activeCoroutine);
            }
        }

        public virtual void DuringActive(BattleVessel user, float timer)
        {
        }
    }
}