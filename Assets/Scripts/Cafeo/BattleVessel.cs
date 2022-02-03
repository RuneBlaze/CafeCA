using System;
using Cafeo.Aimer;
using Cafeo.Castable;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo
{
    [RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(Rigidbody2D))]
    public class BattleVessel : MonoBehaviour
    {
        public AgentSoul soul;
        private Rigidbody2D _body;
        private BoxCollider2D _collider;

        private bool _isPlayer;
        private SpriteRenderer _sprite;

        public UsableItem[] hotbar;
        public int hotbarPointer;
        private AimerGroup _aimer;

        public GenericBrain Brain { get; private set; }

        public enum State
        {
            Idle,
            StartUp,
            Active,
            Stun,
        }

        private void EnterState(State state)
        {
            if (_state == state) return;
            _itemTimer = 0;
            ExitState(_state);
            _state = state;

            if (state == State.Active)
            {
                _activeItem.OnUse(this);
            }

            if (state == State.StartUp && _activeItem.startUp == 0)
            {
                EnterState(State.Active);
            }

            if (state == State.Active && _activeItem.active == 0)
            {
                ApplyActiveItemStun();
            }

            if (state == State.Stun && _stun == 0)
            {
                EnterState(State.Idle);
            }
        }

        public void ActivateItem(UsableItem item)
        {
            _activeItem = item;
            EnterState(State.StartUp);
        }

        public void ActivateItem()
        {
            ActivateItem(hotbar[hotbarPointer]);
        }

        public bool CanTakeAction => _state == State.Idle;

        private void ExitState(State state)
        {
            if (state == State.Active)
            {
                _activeItem = null;
            }
        }

        public RogueManager Scene => RogueManager.Instance;
        
        public void Start()
        {
            Assert.IsNotNull(soul);
            _state = State.Idle;
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _aimer = GetComponent<AimerGroup>();
            Scene.vessels.Add(this);
            hotbar = new UsableItem[10];
            if (Scene.player == this)
            {
                _isPlayer = true;
            }
            SetupMyself();
            DebugColorize();
            DebugSetup();
        }

        private void DebugSetup()
        {
            if (_isPlayer)
            {
                hotbar[0] = new RangedItem();
                SetHotboxPointer(0);
            }
        }

        public UsableItem RetrieveCurItem()
        {
            Assert.IsNotNull(hotbar[hotbarPointer]);
            return hotbar[hotbarPointer];
        }

        public void SetHotboxPointer(int i)
        {
            hotbarPointer = i;
            _aimer.RequestAimer(hotbar[hotbarPointer]);
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
            
            if (_isPlayer) _sprite.color = Color.green;
        }

        private void SetupMyself()
        {
            transform.localScale = soul.HeightScore * Vector3.one;
            _body.mass = soul.Weight;
            Scene.RegisterVessel(this);
            if (_isPlayer)
            {
                Brain = gameObject.AddComponent<PlayerBrain>();
            }
            else
            {
                Brain = gameObject.AddComponent<PlaceholderBrain>();
            }
            Brain.Vessel = this;
        }

        public void Move(Vector2 direction)
        {
            var steer = direction.normalized * 120f;
            var myVel = _body.velocity;
            myVel.x = Mathf.MoveTowards(myVel.x, steer.x, Time.deltaTime * 4f);
            myVel.y = Mathf.MoveTowards(myVel.y, steer.y, Time.deltaTime * 4f);
            _body.velocity = myVel;
        }

        private float _itemTimer = 0;
        private float _stun = 0;
        private State _state;
        private UsableItem _activeItem;

        public void ApplyStun(float duration)
        {
            Assert.IsTrue(duration > 0);
            _stun = Mathf.Max(_stun, duration);
            EnterState(State.Stun);
        }

        public Vector2 CalcArrowSpawnLoc(UsableItem item)
        {
            var radius = soul.HeightScore * 0.5f * Mathf.Sqrt(2);
            var dir = _aimer.CalcDirection(item);
            return (Vector2) transform.position  + dir * radius;
        }

        public Vector2 CalcAimDirection(UsableItem item)
        {
            return _aimer.CalcDirection(item);
        }

        public Vector2 CalcArrowSpawnLoc()
        {
            return CalcArrowSpawnLoc(_activeItem);
        }

        public void RogueUpdate()
        {
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.StartUp:
                    _itemTimer += Time.deltaTime;
                    if (_itemTimer > _activeItem.startUp)
                    {
                        EnterState(State.Active);
                    }
                    break;
                case State.Active:
                    _itemTimer += Time.deltaTime;
                    if (_itemTimer > _activeItem.active)
                    {
                        ApplyActiveItemStun();
                    }
                    else
                    {
                        _activeItem.DuringActive(this, _itemTimer);
                    }
                    break;
                case State.Stun:
                    _stun -= Time.deltaTime;
                    if (_stun <= 0)
                    {
                        _stun = 0;
                        EnterState(State.Idle);
                    }
                    break;
            }
        }

        private void ApplyActiveItemStun()
        {
            if (_activeItem.recovery > 0)
            {
                ApplyStun(_activeItem.recovery);
            }
            else
            {
                EnterState(State.Idle);
            }
        }
    }
}