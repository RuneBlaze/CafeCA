using System;
using System.Collections.Generic;
using System.Linq;
using Cafeo.Aimer;
using Cafeo.Castable;
using Cafeo.Data;
using Cafeo.Entities;
using Cafeo.TestItems;
using Cafeo.UI;
using Cafeo.Utility;
using Cafeo.Utils;
using Drawing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using static Cafeo.Utils.ImDraw;
using Random = UnityEngine.Random;

namespace Cafeo
{
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class BattleVessel : MonoBehaviour, IRogueUpdate
    {
        public AgentSoul soul;
        private Rigidbody2D _body;
        private BoxCollider2D _collider;
        public bool IsPlayer { get; private set; }

        private SpriteRenderer _sprite;

        public UsableItem[] hotbar = new UsableItem[HotbarMax];
        public OneTimeUseItem[] oneTimeUseItems;
        public int hotbarPointer = 0;
        private AimerGroup _aimer;

        public bool IsLeaderAlly => gameObject == RogueManager.Instance.leaderAlly;

        public List<StatusEffect> statusEffects = new();

        public GenericBrain Brain { get; private set; }

        public UnityEvent<State> enterState;
        public UnityEvent onDeath;

        public float dashTimer;
        public float effectTimer;
        public DropInventory drops;
        
        public AimerGroup Aimer => _aimer;

        public Treasure treasure;

        public UnityEvent<Treasure> onGainTreasure;
        public UnityEvent<Treasure> onLoseTreasure;

        public const int HotbarMax = 10;
        public const int TransientMax = 3;

        public enum State
        {
            Idle,
            StartUp,
            Active,
            Stun,
        }
        
        [ShowInInspector]
        public float Atk => soul.Atk + statusEffects.Sum(it => it.atk);
        [ShowInInspector]
        public float Def => soul.Def + statusEffects.Sum(it => it.def);
        [ShowInInspector]
        public float Mat => soul.Mat + statusEffects.Sum(it => it.mat);
        [ShowInInspector]
        public float Mdf => soul.Mdf + statusEffects.Sum(it => it.mdf);
        [ShowInInspector]
        public float Dex => soul.Dex;

        [ShowInInspector]
        public float Spd => 10 + statusEffects.Sum(it => it.spd);

        public float MaxDash => 3f;
        public bool CanDash => dashTimer >= MaxDash;

        public string aiType;
        public void TryDash(Vector2 dir)
        {
            if (!CanDash) return;
            dashTimer = 0;
            AddForce(dir.normalized * 350f);
            ApplyStun(0.5f);
        }

        private void Awake()
        {
            hotbar = new UsableItem[HotbarMax];
            oneTimeUseItems = new OneTimeUseItem[TransientMax];
            statusEffects = new List<StatusEffect>();
            enterState = new UnityEvent<State>();
            drops = new DropInventory();
            
            onGainTreasure = new UnityEvent<Treasure>();
            onLoseTreasure = new UnityEvent<Treasure>();
            
            onDeath = new UnityEvent();
        }

        public void PickupDrop(Collectable collectable)
        {
            // assumes that we are currently in some idle state
            // if (collectable.load is Collectable.TreasureLoad treasureLoad)
            // {
            //     if (treasure != null)
            //     {
            //         treasure.OnUnequip();
            //         treasure.owner = null;
            //     }
            //     else
            //     {
            //         treasure = treasureLoad.treasure;
            //         treasure.owner = this;
            //         treasure.OnEquip();
            //     }
            // } else if (collectable.load is Collectable.BasicsLoad)
            // {
            //     
            // }
        }

        private void EnterState(State state)
        {
            if (this.state == state) return;
            itemTimer = 0;
            ExitState(this.state);
            this.state = state;
            
            enterState.Invoke(state);

            if (state == State.Idle)
            {
                _aimer.locked = false;
            }
            
            if (state == State.StartUp)
            {
                _aimer.locked = true;
                activeItem.OnTryUsing(this);
            }

            if (state == State.Active)
            {
                activeItem.OnUse(this);
                OnUseItem(activeItem);
            }

            if (state == State.StartUp && activeItem.startUp == 0)
            {
                EnterState(State.Active);
            }

            if (state == State.Active && activeItem.active == 0)
            {
                ApplyActiveItemStun();
            }

            if (state == State.Stun && stun == 0)
            {
                EnterState(State.Idle);
            }
        }

        private void OnUseItem(UsableItem item)
        {
            if (item.Announcing) Announce(item);
        }

        public void ActivateItem(UsableItem item, bool secondary = false)
        {
            if (!CanUseItem(item)) return;
            soul.mp -= item.mpCost;
            soul.cp -= item.cpCost;
            item.Setup(this);
            if (!secondary)
            {
                activeItem = item;
                EnterState(State.StartUp);
            }
            else
            {
                StopMoving();
                item.OnUse(this);
            }
        }

        public void ActivateItem()
        {
            ActivateItem(hotbar[hotbarPointer]);
        }

        public bool CanTakeAction => state == State.Idle &&! statusEffects.Any(it => it.paralyzed);

        private void ExitState(State state)
        {
            if (state == State.Active)
            {
                activeItem.OnCounter(this);
                activeItem = null;
            }
        }

        public RogueManager Scene => RogueManager.Instance;

        private void DuplicateSoul()
        {
            var newSoul = gameObject.AddComponent<AgentSoul>();
            soul.CopyTo(newSoul);
            soul = newSoul;
        }
        
        public void Start()
        {
            Assert.IsNotNull(soul);
            if (IsEnemy)
            {
                DuplicateSoul();
            }
            state = State.Idle;
            utilityEnv = gameObject.AddComponent<UtilityEnv>();
            if (IsEnemy) utilityEnv.simple = true;
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _aimer = GetComponent<AimerGroup>();
            // Scene.vessels.Add(this);
            
            if (Scene.player == this)
            {
                IsPlayer = true;
            }
            SetupMyself();
            DebugColorize();
            DebugSetup();
        }

        public void StopMoving()
        {
            // Move(Vector2.zero, 100f);
            _body.velocity = Vector2.MoveTowards(_body.velocity, Vector2.zero, 4f);
            // Debug.Log(_body.velocity);
        }

        private void DebugSetup()
        {
            hotbar = new UsableItem[HotbarMax];
            if (IsPlayer)
            {
                InitPlayerHotBar();
            }
            else
            {
                if (IsAlly)
                {
                    if (aiType == "ranged")
                    {
                        hotbar[0] = new RangedItem
                        {
                            projectileType = new ProjectileType
                            {
                                shape = new ProjectileType.RectShape(0.05f, 0.6f),
                                collidable = true,
                                speed = 17f,
                                density = 50,
                                pierce = 1,
                                bounce = 0,
                                bullet = true,
                            },
                            name = "测试用弓箭",
                        };
                        hotbar[0].AddTag(UsableItem.ItemTag.FreeDPS);
                        hotbar[0].AddTag(UsableItem.ItemTag.Approach);
                    }
                    else
                    {
                        // melee type ally initializer
                        var sword = new MeleeItem(1f, 1f)
                        {
                            name = "测试长剑",
                            meleeType = MeleeItem.MeleeType.BroadSword,
                            active = 0.3f,
                            recovery = 0.05f,
                        };
                        sword.utilityType += (UtilityType) 10f;
                        hotbar[0] = sword;
                        hotbar[0].AddTag(UsableItem.ItemTag.FreeDPS);
                        hotbar[1] = new RangedItem
                        {
                            projectileType = new ProjectileType
                            {
                                shape = new ProjectileType.RectShape(1.2f, 0.23f),
                                collidable = false,
                                speed = 7f,
                                pierce = 3,
                                bounce = 1,
                                timeLimit = 0.4f,
                            },
                            startUp = 0.1f,
                            name = "测试用剑气",
                            withPrimaryShot = true,
                            utilityType = new UtilityType.SingleEnemyInDirection()
                        };
                        hotbar[1].AddTag(UsableItem.ItemTag.FreeDPS);
                    }
                }
                else
                {
                    var rushSkill = new MeleeItem(0.2f, 1)
                    {
                        bodyThrust = 500f,
                        name = "测试用冲刺",
                        active = 0.5f,
                        recovery = 0.5f,
                        meleeType = MeleeItem.MeleeType.BodyRush,
                    };
                    rushSkill.utilityType = new UtilityType.Disregarding();
                    hotbar[1] = rushSkill;
                    hotbar[1].AddTag(UsableItem.ItemTag.FreeDPS);
                    SetHotboxPointer(1);
                }
            }

            if (IsPlayer)
            {
                _aimer.autoAim = false;
            }
        }

        private void InitPlayerHotBar()
        {
            hotbar[0] = new RangedItem { name = "以撒的眼泪" };
            hotbar[1] = new MeleeItem(0.3f, 1f) { name = "匕首" };
            hotbar[2] = new TossItem { name = "测试用投掷道具" };
            hotbar[2].SetHitAllies();
            hotbar[4] = new RangedItem { name = "Fanshot 测试", fan = 3, spread = 90 };

            hotbar[6] = new MeleeItem(1f, 1f) { name = "测试用锤子", meleeType = MeleeItem.MeleeType.Hammer };
            hotbar[7] = new MeleeItem(1f, 1f) { name = "测试用长矛", meleeType = MeleeItem.MeleeType.Thrust };
            hotbar[8] = new RangedItem
            {
                projectileType = new ProjectileType
                {
                    shape = new ProjectileType.RectShape(0.05f, 0.6f),
                    collidable = true,
                    speed = 9f,
                    pierce = 1,
                    bounce = 1,
                },
                fan = 3,
                spread = 60,
                shots = 3,
                duration = 0.5f,
                name = "测试用散射针",
            };

            hotbar[9] = new MeleeItem(1f, 1f)
            {
                name = "测试镰刀",
                meleeType = MeleeItem.MeleeType.Scythe,
                active = 0.5f,
                recovery = 0.1f,
            };

            hotbar[5] = new MeleeItem(1f, 1f)
            {
                name = "测试大剑",
                meleeType = MeleeItem.MeleeType.GreatSword,
                active = 0.5f,
                recovery = 0.1f,
            };

            var sword = new MeleeItem(1f, 1f)
            {
                name = "测试长剑",
                meleeType = MeleeItem.MeleeType.BroadSword,
                active = 0.3f,
                recovery = 0.05f,
            };

            var testBoomerang = new RangedItem
            {
                projectileType = new ProjectileType
                {
                    shape = new ProjectileType.PredefinedShape("Boomerang").Rescale(0.2f),
                    collidable = true,
                    speed = 6f,
                    pierce = 30,
                    bounce = 30,
                    initialSpin = 360,
                    bounciness = 1f,
                    boomerang = 1f,
                },
                shots = 1,
                name = "测试用回旋镖",
            };

            var testGun = new RangedItem
            {
                projectileType = new ProjectileType()
                {
                    shape = new ProjectileType.CircleShape(0.03f),
                    collidable = true,
                    speed = 15f,
                    pierce = 1,
                    bounce = 0,
                    bullet = true,
                },
                shots = 1,
                name = "测试用枪",
                active = 0f,
                recovery = 0.05f,
                instability = 20,
                mpCost = 5,
            };
            hotbar[0] = testGun;

            var rushSkill = new MeleeItem(0.2f, 1)
            {
                bodyThrust = 800f,
                name = "测试用冲刺",
                active = 0.5f,
                recovery = 0.1f,
                meleeType = MeleeItem.MeleeType.BodyRush,
            };
            string preset = "gun";
            if (preset == "sword")
            {
                hotbar[0] = sword;
                hotbar[1] = new RangedItem
                {
                    projectileType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(1.2f, 0.23f),
                        collidable = false,
                        speed = 7f,
                        pierce = 3,
                        bounce = 1,
                        timeLimit = 0.4f,
                    },
                    startUp = 0.1f,
                    name = "测试用剑气",
                    withPrimaryShot = true,
                };
            } else if (preset == "bow")
            {
                hotbar[0] = new RangedItem
                {
                    projectileType = new ProjectileType
                    {
                        shape = new ProjectileType.RectShape(0.05f, 0.6f),
                        collidable = true,
                        speed = 17f,
                        density = 50,
                        pierce = 1,
                        bounce = 0,
                        bullet = true,
                    },
                    name = "测试用弓箭",
                };
                
                hotbar[1] = new RangedItem
                {
                    projectileType = ((RangedItem) hotbar[0]).projectileType,
                    duration = 0.5f,
                    shots = 4,
                    name = "连射",
                    startUp = 0.7f,
                };
            } else if (preset == "gun")
            {
                hotbar[0] = testGun;
                hotbar[1] = new TossItem()
                {
                    maxDistance = 0,
                    coroutineFactory = SkillPresets.GunnerRegen,
                    name = "装填",
                    // active = Single.PositiveInfinity,
                };
                hotbar[2] = new RangedItem
                {
                    projectileType = new ProjectileType()
                    {
                        shape = new ProjectileType.CircleShape(0.03f),
                        collidable = true,
                        speed = 15f,
                        pierce = 1,
                        bounce = 0,
                        bullet = true,
                    },
                    shots = 40,
                    duration = 3f,
                    name = "扫射",
                    active = 0f,
                    recovery = 0.5f,
                    instability = 30,
                    mpCost = 5,
                };
            }
            hotbar[9] = new TossItem
            {
                name = "测试用自我中心 Buff", maxDistance = 0, alwaysSplash = true,
                power = 0,
            };
            hotbar[9].hitEffects.buffs.Add(
                new HitEffects.BuffExpr(HitEffects.SecondaryAttr.Atk, "0.2 * mat", 5f));
            hotbar[9].SetHitAllies();
            hotbar[8] = new TossItem
            {
                name = "测试用回复", maxDistance = 0, alwaysSplash = true,
                damageType = UsableItem.DamageType.HpRecovery,
                power = 30,
            };
            hotbar[8].SetHitAllies();
            SetHotboxPointer(9);
        }

        public void UsePrimaryShot()
        {
            ActivateItem(hotbar[0], true);
        }

        public UsableItem RetrieveCurItem()
        {
            // Assert.IsNotNull(hotbar[hotbarPointer]);
            return hotbar[hotbarPointer];
        }

        public void SetHotboxPointer(int i)
        {
            hotbarPointer = i;
            hotbar[hotbarPointer].Setup(this);
            _aimer.RequestAimer(hotbar[hotbarPointer]);
        }

        public void TrySetHotboxPointer(int i)
        {
            if (hotbar[i] == null)
            {
                return;
            }
            
            SetHotboxPointer(i);
        }

        private void DebugColorize()
        {
            if (soul.alignment > 0)
            {
                _sprite.color = Color.blue;
            }
            else
            {
                _sprite.color = Color.yellow / 2 + Color.red;
            }
            
            if (IsPlayer) _sprite.color = Color.green;
        }
        
        public bool IsAlly => soul.alignment > 0;
        public bool IsEnemy => !IsAlly;

        private void SetupMyself()
        {
            transform.localScale = soul.HeightScore * Vector3.one;
            _body.mass = soul.Weight;
            if (IsAlly)
            {
                gameObject.layer = LayerMask.NameToLayer("Allies");
            }
            if (IsEnemy)
            {
                gameObject.layer = LayerMask.NameToLayer("Enemies");
            }
            gameObject.tag = IsAlly ? "Ally" : "Enemy";
            Scene.RegisterVessel(this);
            if (IsPlayer)
            {
                Brain = gameObject.AddComponent<PlayerBrain>();
            }
            else
            {
                Brain = gameObject.AddComponent<PlaceholderBrain>();
            }
            Brain.Vessel = this;
            dashTimer = MaxDash;

            SyncPhysics();
        }

        private void SyncPhysics()
        {
            _body.drag = 1.5f / NormalizedSpd;
        }

        private float NormalizedSpd => Spd / 10;

        public void Move(Vector2 direction, float lerp = 4f)
        {
            var steer = direction.normalized * 120f * NormalizedSpd;
            var myVel = _body.velocity;
            var angleDiff = Mathf.Sqrt(Mathf.Abs(Vector2.SignedAngle(direction, myVel)) / 180f);
            lerp *= NormalizedSpd;
            myVel.x = Mathf.MoveTowards(myVel.x, steer.x, Time.deltaTime * lerp * (1 + angleDiff));
            myVel.y = Mathf.MoveTowards(myVel.y, steer.y, Time.deltaTime * lerp * (1 + angleDiff));
            _body.velocity = myVel;
        }

        private float itemTimer = 0;
        private float stun = 0;
        private float invincible = 0;
        private State state;
        private UsableItem activeItem;
        private UtilityEnv utilityEnv;
        public UtilityEnv UtilityEnv => utilityEnv;

        public void ApplyStun(float duration)
        {
            Assert.IsTrue(duration > 0);
            if (state == State.Active && activeItem.isArts)
            {
                return;
            }

            if (state == State.Active && itemTimer < activeItem.active)
            {
                activeItem.OnInterrupt(this);
            }
            stun = Mathf.Max(stun, duration);
            EnterState(State.Stun);
        }
        
        public void ApplyInvincible(float duration)
        {
            Assert.IsTrue(duration > 0);
            invincible = Mathf.Max(invincible, duration);
        }

        public Vector2 CalcArrowSpawnLoc(UsableItem item)
        {
            var radius = soul.HeightScore * 0.1f * Mathf.Sqrt(2);
            var dir = _aimer.CalcDirection(item);
            return (Vector2) transform.position  + dir * radius;
        }
        
        public float Radius => soul.HeightScore * 0.5f * Mathf.Sqrt(2);
        public float halfSideLength => soul.HeightScore * 0.5f;

        public Vector2 CalcAimDirection(UsableItem item)
        {
            return _aimer.CalcDirection(item);
        }

        public BattleVessel CalcAimTarget(UsableItem item)
        {
            return _aimer.CalcTargetObject(item)?.GetComponent<BattleVessel>();
        }

        public Vector2 CalcArrowSpawnLoc()
        {
            return CalcArrowSpawnLoc(activeItem);
        }
        
        public void AddStatus(StatusEffect status)
        {
            int curCount = statusEffects.Count(it => 
                it.statusTag != null && it.statusTag.CompareStatusTag(status.statusTag));
            if (curCount < status.maxStack)
            {
                statusEffects.Add(status);
                status.OnAdd();
                Scene.CreatePopup(transform.position, status.displayName, Palette.milkYellow);
            }
            else
            {
                var existing = statusEffects.OrderBy(it => it.timer).First(it =>
                    it.statusTag != null && it.statusTag.CompareStatusTag(status.statusTag));
                existing.duration = status.duration;
                existing.timer = 0;
            }
        }

        public void RemoveStatus(Predicate<StatusEffect> pred)
        {
            foreach (var statusEffect in statusEffects)
            {
                if (pred(statusEffect))
                {
                    statusEffect.OnEnd();
                }
            }

            statusEffects.RemoveAll(pred);
        }

        private void OnStatusEffectExpired(StatusEffect statusEffect)
        {
            Scene.CreatePopup(transform.position, $"- {statusEffect.displayName}", Palette.gray);
        }

        private void Announce(UsableItem item)
        {
            Scene.CreatePopup(transform.position, item.name, Palette.milkYellow);
        }

        public void RogueUpdate()
        {
            SyncPhysics();
            for (int i = statusEffects.Count - 1; i >= 0; i--)
            {
                var effect = statusEffects[i];
                effect.Update();
                if (effect.Finished)
                {
                    OnStatusEffectExpired(effect);
                    effect.OnEnd();
                    statusEffects.RemoveAt(i);
                }
            }

            invincible -= Time.deltaTime;
            invincible = Mathf.Max(invincible, 0);

            switch (state)
            {
                case State.Idle:
                    break;
                case State.StartUp:
                    itemTimer += Time.deltaTime;
                    if (itemTimer > activeItem.startUp)
                    {
                        EnterState(State.Active);
                    }
                    break;
                case State.Active:
                    itemTimer += Time.deltaTime;
                    if (itemTimer > activeItem.active)
                    {
                        ApplyActiveItemStun();
                    }
                    else
                    {
                        activeItem.DuringActive(this, itemTimer);
                    }
                    break;
                case State.Stun:
                    stun -= Time.deltaTime;
                    if (stun <= 0)
                    {
                        stun = 0;
                        EnterState(State.Idle);
                    }
                    break;
            }

            dashTimer += Time.deltaTime;
            dashTimer = Mathf.Clamp(dashTimer, 0, MaxDash);

            if (soul.Dead)
            {
                onDeath.Invoke();
                Destroy(gameObject);
            }

            for (int i = 0; i < HotbarMax; i++)
            {
                if (hotbar[i]?.ShouldDiscard == true)
                {
                    hotbar[i] = null;
                }
            }

            UpdatePassiveEffects();
        }

        private void UpdatePassiveEffects()
        {
            if (Mathf.RoundToInt(effectTimer) != Mathf.RoundToInt(effectTimer + Time.deltaTime))
            {
                foreach (var status in statusEffects)
                {
                    status.passiveEffect?.EverySec(this);
                }
            }
            effectTimer += Time.deltaTime;
            foreach (var status in statusEffects)
            {
                status.passiveEffect?.EveryTick(this);
            }
        }

        public void ApplyActiveItemStun()
        {
            activeItem.OnEndUse(this);
            if (activeItem.recovery > 0)
            {
                ApplyStun(activeItem.recovery);
            }
            else
            {
                EnterState(State.Idle);
            }
        }

        public void ApplyDamage(int damage, float stun, Vector2 knockback, 
            AgentSoul.ResourceType resourceType = AgentSoul.ResourceType.Hp)
        {
            if (invincible > 0) return;
            if (stun > 0)
            {
                ApplyStun(stun);
                ApplyInvincible(stun/2);
            }
            if (knockback.magnitude > 0)
            {
                _body.AddForce(knockback, ForceMode2D.Impulse);
            }
            damage = ModifyDamage(damage);
            GainCp(3);
            soul.TakeDamage(damage, resourceType);
            Scene.CreatePopup(transform.position, $"{damage}", Palette.red);
        }

        public int ModifyDamage(int damage)
        {
            var shield = statusEffects.Sum(x => x.shield);
            var shieldPerc = statusEffects.Sum(x => x.shieldPerc);
            return Mathf.RoundToInt((damage - shield) * (1 - shieldPerc));
        }

        public void ApplyDamage(float damage, float stun, Vector2 knockback)
        {
            ApplyDamage(Mathf.RoundToInt(damage), stun, knockback);
        }

        public void ApplyHeal(int amount)
        {
            soul.Heal(amount);
            Scene.CreatePopup(transform.position, $"{amount}", Palette.green);
        }

        public void ApplyHealMp(int amount)
        {
            soul.HealMp(amount);
            Scene.CreatePopup(transform.position, $"{amount}", Palette.skyBlue);
        }

        public void ApplyHealCp(int amount)
        {
            soul.HealCp(amount);
            Scene.CreatePopup(transform.position, $"{amount}", Palette.green);
        }

        public void GainCp(int cnt)
        {
            soul.HealCp(cnt);
        }

        public float BodyDistance(BattleVessel other)
        {
            return Mathf.Max(0,Vector3.Distance(transform.position, other.transform.position) - other.Radius -
                               Radius);
        }

        public void AddForce(Vector2 force)
        {
            _body.AddForce(force, ForceMode2D.Impulse);
        }

        private void LateUpdate()
        {
            // draw debug gauge
            if (IsEnemy)
            {
                float width = 1.3f;
                var position = transform.position;
                var x = position.x;
                var y = position.y;
                DrawGauge(Draw.ingame, x - width/2, y + halfSideLength + 0.2f, width, 0.15f, 
                    soul.hp, soul.MaxHp, Palette.purple, Palette.red, 0f);
            }
        }
        
        public bool CanUseItem(UsableItem item)
        {
            if (item.HasTag(UsableItem.ItemTag.Dash))
            {
                return CanDash;
            }
            return soul.mp >= item.mpCost && soul.cp >= item.cpCost;
        }
        
        public bool IsFacing(BattleVessel other, float tol)
        {
            Vector2 pos = other.transform.position - transform.position;
            var delta = VectorUtils.DegreesBetween(pos.normalized, _aimer.RangedAimer.transform.right);
            return Mathf.Abs(delta) < tol;
        }

        public bool TryGainOneTimeUse(OneTimeUseItem item)
        {
            for (int i = 0; i < TransientMax; i++)
            {
                if (hotbar[7 + i] == null)
                {
                    hotbar[7 + i] = item;
                    return true;
                }
            }
            return false;
        }

        public bool IsOneTimeUseFull
        {
            get
            {
                for (int i = 0; i < TransientMax; i++)
                {
                    if (hotbar[7 + i] == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void DropTreasure()
        {
            Assert.IsNotNull(treasure);
            treasure.OnDrop(this);
            onLoseTreasure.Invoke(treasure);
            Scene.SpawnDroppable(transform.position, treasure, 
                VectorUtils.OnUnitCircle(Random.Range(0, 4 * Mathf.PI)));
            treasure = null;
        }

        public void Kill()
        {
            soul.hp = 0;
        }
        
        public bool HasTreasure => treasure != null;
    }
}