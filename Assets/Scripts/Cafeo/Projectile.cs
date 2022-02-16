using System;
using DG.Tweening;
using Drawing;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        private bool destroyOnNextFrame;

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
            Vector2 initialFacing = type.initialFacing == Vector2.zero ? initialDirection.normalized : type.initialFacing;
            var angle = Mathf.Atan2(initialFacing.y, initialFacing.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            _collider.density = type.density;
            _body.useAutoMass = true;
            // _body.inertia = 30f;
            // Debug.Log(transform.rotation.eulerAngles);
            // Quaternion.AngleAxis(-angle / 2f + inc * i, Vector3.forward) * direction.normalized;
            _body.gravityScale = 0;
            if (bounce > 0)
            {
                var mat2d = new PhysicsMaterial2D
                {
                    friction = 0,
                    bounciness = 0.8f
                };
                _body.sharedMaterial = mat2d;
            }
            _collider.isTrigger = !type.collidable;
            if (type.maxSize > 0)
            {
                curSize = 0.05f;
            }
            _timer = 0;
            transform.position = transform.position + (Vector3) (type.speed * initialDirection.normalized * 0.07f);
            RogueManager.Instance.rogueUpdateEvent.AddListener(RogueUpdate);
        }

        private void SyncTransform()
        {
            _collider.isTrigger = !type.collidable;
            if (sizeDelta != 0)
            {
                curSize += sizeDelta * Time.deltaTime;
            }

            if (type.maxSize > 0)
            {
                curSize += 4.5f * Time.deltaTime;
                // Debug.Log(curSize);
                if (curSize > type.maxSize)
                {
                    curSize = type.maxSize;
                    destroyOnNextFrame = true;
                }
            }
            
            if (curSize > 0)
            {
                transform.localScale = new Vector3(curSize, curSize, curSize);
            }

            if (curSize <= 0)
            {
                SelfDestruct();
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
            switch (type.shape)
            {
                case ProjectileType.SquareShape squareShape:
                    draw.CircleXY( transform.position, squareShape.size/2 * curSize);
                    break;
                case ProjectileType.CircleShape circleShape:
                    draw.CircleXY(transform.position, circleShape.radius * curSize);
                    break;
                case ProjectileType.RectShape rectShape:
                    var collider = _collider as BoxCollider2D;
                    var size = collider.size;
                    var topLeft = transform.TransformPoint ( collider.offset  + new Vector2(-size.x/2, size.y/2));
                    var topRight = transform.TransformPoint ( collider.offset  + new Vector2(size.x/2, size.y/2));
                    var btmLeft = transform.TransformPoint ( collider.offset  + new Vector2(-size.x/2, -size.y/2));
                    var btmRight = transform.TransformPoint ( collider.offset  + new Vector2(size.x/2, -size.y/2));
                    // // float btm = worldPos.y - (size.y / 2f);
                    // // float left = worldPos.x - (size.x / 2f);
                    // // float right = worldPos.x + (size.x /2f);
                    // var topLeft = new Vector3( left, top, worldPos.z);
                    // var topRight = new Vector3( right, top, worldPos.z);
                    // var btmLeft = new Vector3( left, btm, worldPos.z);
                    // var btmRight = new Vector3( right, btm, worldPos.z);
                    rectShape.vertices[0] = topLeft;
                    rectShape.vertices[1] = topRight;
                    rectShape.vertices[2] = btmRight;
                    rectShape.vertices[3] = btmLeft;
                    draw.Polyline(rectShape.vertices, true);
                    break;
                case ProjectileType.CustomShape customShape:
                    var polycollider = _collider as PolygonCollider2D;
                    for (int i = 0; i < polycollider.points.Length; i++)
                    {
                        var point = polycollider.points[i];
                        var transformedPoint = transform.TransformPoint(point + polycollider.offset);
                        customShape.vertices[i] = transformedPoint;
                    }
                    draw.Polyline(customShape.vertices, true);
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
            if (go.layer == LayerMask.NameToLayer("Obstacle"))
            {
                DecBounce();
                // _body.AddTorque(10f);
            }
            if (IsBattleVesselGameObject(go))
            {
                var vessel = go.GetComponent<BattleVessel>();
                OnHit(vessel);
            }
        }

        private void DecBounce()
        {
            if (bounce > 0)
            {
                bounce--;
            }
            else
            {
                SelfDestruct();
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
            if (destroyOnNextFrame)
            {
                SelfDestruct();
                return;
            }
            _timer += Time.deltaTime;
            if (_timer >= 20 || _timer > type.timeLimit)
            {
                SelfDestruct();
            }
            _traveledDistance += _body.velocity.magnitude * Time.deltaTime;
            if (distanceLimit >= 0 && _traveledDistance > distanceLimit)
            {
                SelfDestruct();
            }
            SyncTransform();

            // if (_timer >= 0.1f)
            // {
            //     if (type.collidable) _collider.isTrigger = true;
            // }
        }
    }
}