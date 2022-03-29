using System;
using System.Collections;
using System.Collections.Generic;
using Cafeo.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Cafeo.Castable
{
    [Serializable]
    public class UsableItem : AbstractItem
    {
        public float startUp;
        public float active;
        public float recovery = 0.5f;

        public float cd;
        public bool isArts;
        public bool startUpMovable;
        public float baseDamage;

        public int orbit = 1;
        public int orientation = 0;

        public bool hitAllies;
        public bool hitEnemies = true;

        public float power = 100f;
        public float hitStun = 0.2f;

        public UtilityType utilityType;

        public UsableItem()
        {
            damageType = DamageType.Null;
            powerType = PowerType.Physical;
            hitEffects = new HitEffects();
        }

        public enum PowerType
        {
            Physical,
            Ranged,
            Magic,
        }

        public enum DamageType
        {
            HpDamage,
            MpDamage,
            HpRecovery,
            Null,
        }

        public PowerType powerType;
        public DamageType damageType;

        public int mpCost;
        public int cpCost;
        public float knockbackPower = 1;
        public bool stopOnUse;
        public UnityEvent onCounter;
        public HitEffects hitEffects;

        public static UsableItem dashSkill = new()
        {
            tags = { ItemTag.Approach, ItemTag.Dash },
            utilityType = new UtilityType.DashUtility(),
        };

        public enum ItemTag
        {
            Approach, // this skill is useful for approaching
            StapleDPS, // for dealing damage reliably
            FreeDPS, // for easy damage
            Dash,
        }
        
        public List<ItemTag> tags = new();
        
        /* <summary>If this skill upon use, should have UI indication that it has been used.</summary>*/
        public bool Announcing => !tags.Contains(ItemTag.FreeDPS) && !tags.Contains(ItemTag.Dash);

        public void AddTag(ItemTag tag)
        {
            tags.Add(tag);
        }
        
        public bool HasTag(ItemTag tag)
        {
            return tags.Contains(tag);
        }

        public Func<BattleVessel, IEnumerator> coroutineFactory;
        public IEnumerator coroutineOnStart;
        
        public RogueManager Scene => RogueManager.Instance;
        public int targetLayerMask = -1;
        public string targetTag;
        private bool coroutineDone;
        private Coroutine activeCoroutine;
        
        // called when the skill is being switched to
        public virtual void Setup(BattleVessel user)
        {
            onCounter = new UnityEvent();
            
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

            if (hitAllies)
            {
                user.Aimer.TossAimer.BehaviorTree.SetVariableValue("TargetObject", 
                    user.UtilityEnv.targetAlly != null ? user.UtilityEnv.targetAlly.gameObject : null);
            }
            else
            {
                user.Aimer.TossAimer.BehaviorTree.SetVariableValue("TargetObject", user.Brain.RetrieveTargetEnemy()?.gameObject);
            }
        }

        private IEnumerator WrappedCoroutine(BattleVessel user)
        {
            yield return coroutineOnStart;
            coroutineDone = true;
            user.ApplyActiveItemStun();
            yield return null;
        }

        public virtual void OnTryUsing(BattleVessel user)
        {
            utilityType?.OnTryUsing(user);
        }

        public virtual void OnUse(BattleVessel user)
        {
            utilityType?.OnUse(user);
            user.UtilityEnv.justUsed = this;
            if (coroutineFactory != null)
            {
                coroutineOnStart = coroutineFactory.Invoke(user);
            }
            
            if (coroutineOnStart != null)
            {
                coroutineDone = false;
                active = float.PositiveInfinity;
                activeCoroutine = user.StartCoroutine(WrappedCoroutine(user));
            }

            orientation++;
            orientation %= orbit;
            
            if (stopOnUse) user.StopMoving();
        }

        public virtual void OnCounter(BattleVessel user)
        {
            
        }

        public virtual void OnInterrupt(BattleVessel user)
        {
            onCounter.Invoke();
            if (activeCoroutine != null)
            {
                user.StopCoroutine(activeCoroutine);
            }
        }

        public virtual void DuringActive(BattleVessel user, float timer)
        {
        }
        
        public virtual void OnEndUse(BattleVessel user)
        {
            
        }

        public virtual void ApplyHitEffects(BattleVessel user, BattleVessel target)
        {
            hitEffects.Apply(user, target, levelOffset: Level - 1);
            // foreach (var buff in hitEffects.buffs)
            // {
            //     var statusEffect = buff.CalcStatus(user, target);
            //     target.AddStatus(statusEffect);
            // }
        }

        public void SetHitEnemies()
        {
            hitAllies = false;
            hitEnemies = true;
        }

        public void SetHitAllies()
        {
            hitAllies = true;
            hitEnemies = false;
        }

        public virtual void ApplyEffect(BattleVessel user, BattleVessel target, Vector2 hitSource, Projectile hitProj)
        {
            ApplyHitEffects(user, target);
        }

        public virtual void ApplyCalculatedDamage(BattleVessel user, BattleVessel target, float stun, Vector2 knockback)
        {
            // deal HP, deal MP damage, etc.
            if (damageType == DamageType.HpRecovery && power > 0)
            {
                var heal = Scene.CalculateHeal(user, target, this);
                target.ApplyHeal(Mathf.RoundToInt(heal));
            }
            else
            {
                var damage = powerType switch
                {
                    PowerType.Physical => Scene.CalculateDamageMelee(user, target, this),
                    PowerType.Ranged => Scene.CalculateDamageRanged(user, target, this),
                    _ => Scene.CalculateDamageMelee(user, target, this, true)
                };

                switch (damageType)
                {
                    case DamageType.Null:
                        break;
                    case DamageType.HpDamage:
                        target.ApplyDamage(Mathf.RoundToInt(damage), stun, knockback, AgentSoul.ResourceType.Hp);
                        break;
                    case DamageType.MpDamage:
                        target.ApplyDamage(Mathf.RoundToInt(damage), stun, knockback, AgentSoul.ResourceType.Mp);
                        break;
                    case DamageType.HpRecovery:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual bool IsGonnaHit(BattleVessel user, BattleVessel target)
        {
            return true;
        }

        public void ApplyEffect(BattleVessel user, BattleVessel target)
        {
            ApplyEffect(user, target, Vector2.zero, null);
        }
    }
}