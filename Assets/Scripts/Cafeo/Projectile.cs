using System;
using Drawing;
using UnityEngine;
using UnityEngine.Events;

namespace Cafeo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IRogueUpdate
    {
        public bool alignment;
        public int pierce = 1;
        public int bounce = 0;
        public BattleVessel owner;
        public Vector2 initialDirection;

        public ProjectileType type;
        private Rigidbody2D _body;
        private Collider2D _collider;
        private float _timer;
        private float _traveledDistance;

        public float sizeDelta = 0;
        private float curSize = 1;
        public float distanceLimit = -1;
        public UnityEvent beforeDestroy = new ();

        private void Start()
        {
            
        }

        public void Setup()
        {
            // Called after type is set on creation
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            SetupLayer();
            _body.velocity = initialDirection.normalized * type.speed;
            _body.gravityScale = 0;
            if (!type.collidable)
            {
                _collider.isTrigger = true;
            }
            _timer = 0;
            RogueManager.Instance.rogueUpdateEvent.AddListener(RogueUpdate);
        }

        private void SyncTransform()
        {
            if (sizeDelta != 0)
            {
                curSize += sizeDelta * Time.deltaTime;
                if (curSize > 0)
                {
                    transform.localScale = new Vector3(curSize, curSize, curSize);
                }
                else
                {
                    SelfDestruct();
                }
            }
        }

        private void SetupLayer()
        {
            if (type.hitAllies && type.hitEnemies)
            {
                gameObject.layer = LayerMask.NameToLayer("HitBoth");
            }
            else if (type.hitAllies)
            {
                gameObject.layer = LayerMask.NameToLayer(owner.soul.alignment > 0 ? "HitAlly" : "HitEnemy");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer(owner.soul.alignment > 0 ? "HitEnemy" : "HitAlly");
            }
        }

        private void Update()
        {
            var draw = Draw.ingame;
            switch (type.Shape)
            {
                case ProjectileType.SquareShape squareShape:
                    draw.CircleXY( transform.position, squareShape.size/2 * curSize);
                    break;
                case ProjectileType.CircleShape circleShape:
                    draw.CircleXY(transform.position, circleShape.radius * curSize);
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnAnyCollide(col);
        }

        private void OnAnyCollide(Collider2D col)
        {
            var go = col.gameObject;
            if (IsBattleVesselGameObject(go))
            {
                var vessel = go.GetComponent<BattleVessel>();
                OnHit(vessel);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            OnAnyCollide(col.collider);
        }
        
        private bool IsBattleVesselGameObject(GameObject go)
        {
            return go.layer == LayerMask.NameToLayer("Allies") || go.layer == LayerMask.NameToLayer("Enemies");
        }

        private void OnHit(BattleVessel target)
        {
            ResolveHitEffect(target);
            pierce--;
            if (pierce == 0)
            {
                SelfDestruct();
            }
        }

        private void ResolveHitEffect(BattleVessel target)
        {
            target.ApplyDamage(1, 0.5f, _body.velocity * 10);
        }

        private void SelfDestruct()
        {
            beforeDestroy.Invoke();
            Destroy(gameObject);
        }

        public void RogueUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= 20)
            {
                SelfDestruct();
            }
            _traveledDistance += _body.velocity.magnitude * Time.deltaTime;
            if (distanceLimit >= 0 && _traveledDistance > distanceLimit)
            {
                SelfDestruct();
            }
            SyncTransform();
        }
    }
}