﻿using Cafeo.Castable;
using Cafeo.Utils;
using DG.Tweening;
using Drawing;
using UnityEngine;
using UnityEngine.Events;

namespace Cafeo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IRogueUpdate
    {
        public bool alignment;
        public int pierce = 1231231234;
        public int bounce = -1;
        public BattleVessel owner;
        public Vector2 initialDirection;

        public ProjectileType type;

        public bool isMelee;

        public UsableItem parentSkill;

        public float deltaSize;
        public float distanceLimit = -1;
        public UnityEvent beforeDestroy = new();

        public int contactLayer;

        public UnityEvent<BattleVessel> onHit;
        private Rigidbody2D _body;
        private Collider2D _collider;
        private float _timer;
        private float _traveledDistance;
        private float curSize = 1;

        private bool destroyOnNextFrame;
        private int totalBounce;

        public Vector2 Velocity => _body.velocity;

        private void Start()
        {
            if (type.rotate != null)
            {
                var orient = type.rotate.orientation ? 1 : -1;
                var initialAngle = transform.rotation.eulerAngles.z - type.rotate.range * orient * 0.5f;
                var finalAngle = transform.rotation.eulerAngles.z + type.rotate.range * orient * 0.5f;
                var rotateY = 0;
                transform.localScale = new Vector3(-orient, 1, 1);
                transform.rotation = Quaternion.Euler(0, rotateY, initialAngle);
                var tween = transform.DORotate(
                    new Vector3(0, rotateY, finalAngle),
                    type.rotate.speed);
                tween.onComplete += SelfDestruct;
            }

            if (type.initialSpin != 0) _body.AddTorque(type.initialSpin);
        }

        private void LateUpdate()
        {
            var draw = Draw.ingame;
            switch (type.shape)
            {
                case ProjectileType.SquareShape squareShape:
                    draw.CircleXY(transform.position, squareShape.size / 2 * curSize);
                    break;
                case ProjectileType.CircleShape circleShape:
                    draw.CircleXY(transform.position, circleShape.radius * curSize);
                    break;
                case ProjectileType.RectShape rectShape:
                    var collider = _collider as BoxCollider2D;
                    var size = collider.size;
                    var topLeft = transform.TransformPoint(collider.offset + new Vector2(-size.x / 2, size.y / 2));
                    var topRight = transform.TransformPoint(collider.offset + new Vector2(size.x / 2, size.y / 2));
                    var btmLeft = transform.TransformPoint(collider.offset + new Vector2(-size.x / 2, -size.y / 2));
                    var btmRight = transform.TransformPoint(collider.offset + new Vector2(size.x / 2, -size.y / 2));
                    rectShape.vertices[0] = topLeft;
                    rectShape.vertices[1] = topRight;
                    rectShape.vertices[2] = btmRight;
                    rectShape.vertices[3] = btmLeft;
                    draw.Polyline(rectShape.vertices, true);
                    break;
                case ProjectileType.CustomShape customShape:
                    var polycollider = _collider as PolygonCollider2D;
                    for (var i = 0; i < polycollider.points.Length; i++)
                    {
                        var point = polycollider.points[i];
                        var transformedPoint = transform.TransformPoint(point + polycollider.offset);
                        customShape.vertices[i] = transformedPoint;
                    }

                    draw.Polyline(customShape.vertices, true);
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            OnAnyCollide(col.collider);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnAnyCollide(col);
        }

        public void RogueUpdate()
        {
            if (destroyOnNextFrame)
            {
                SelfDestruct();
                return;
            }

            _timer += Time.deltaTime;
            if (type.acceleration > 0)
            {
                var velocity = _body.velocity;
                velocity += velocity.normalized * type.acceleration * Time.deltaTime;
                _body.velocity = velocity;
                if (_body.velocity.magnitude > type.maxSpeed)
                    _body.velocity = _body.velocity.normalized * type.maxSpeed;
            }

            if (_timer >= 20 || _timer > type.timeLimit) SelfDestruct();
            _traveledDistance += _body.velocity.magnitude * Time.deltaTime;
            if (distanceLimit >= 0 && _traveledDistance > distanceLimit) SelfDestruct();

            if (type.followOwner) transform.position = owner.transform.position;

            if (type.boomerang > 0)
            {
                // var t = _timer * 24;
                // var f = new Vector2(Mathf.Cos(t), Mathf.Sin(t));
                var diff = owner.transform.position - transform.position;
                var f = diff.normalized * Mathf.Pow(_timer * 12f, 1.5f);
                _body.AddForce(f);
                if (Mathf.Abs(_body.angularVelocity) < 360) _body.AddTorque(Mathf.Sign(_body.angularVelocity) * 5f);
                if (_timer > 0.5f && diff.sqrMagnitude < 0.5f) SelfDestruct();
            }

            if (type.homingStrength > 0)
            {
                var homingTarget = Physics2D.OverlapCircle(
                    transform.position,
                    type.homingRadius,
                    contactLayer
                );
                if (homingTarget != null)
                    VectorUtils.RotateTowards(_body, homingTarget.transform.position, type.homingStrength);
            }

            SyncTransform();
        }

        public void RegisterMeleeOwner(UsableItem item)
        {
            isMelee = true;
            item.onCounter.AddListener(SelfDestruct);
        }

        public void Setup()
        {
            // Called after type is set on creation
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            onHit = new UnityEvent<BattleVessel>();
            SetupLayer();
            if (type.followOwner)
            {
                // we do nothing here
            }
            else
            {
                _body.velocity = initialDirection.normalized * type.speed;
            }

            var initialFacing = type.initialFacing == Vector2.zero ? initialDirection.normalized : type.initialFacing;
            var angle = Mathf.Atan2(initialFacing.y, initialFacing.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            _body.useAutoMass = true;
            _collider.density = type.density;
            // _body.inertia = 30f;
            // Debug.Log(transform.rotation.eulerAngles);
            // Quaternion.AngleAxis(-angle / 2f + inc * i, Vector3.forward) * direction.normalized;
            _body.gravityScale = 0;
            if (bounce > 0)
            {
                var mat2d = new PhysicsMaterial2D
                {
                    friction = 0,
                    bounciness = type.bounciness
                };
                _body.sharedMaterial = mat2d;
            }

            _collider.isTrigger = !type.collidable;
            if (type.maxSize > 0) curSize = 0.05f;
            _timer = 0;
            transform.position = transform.position + (Vector3)(type.speed * initialDirection.normalized * 0.07f);
            RogueManager.Instance.rogueUpdateEvent.AddListener(RogueUpdate);

            if (type.kineticBody) _body.freezeRotation = true;

            if (type.bullet) _body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            contactLayer = VectorUtils.GetCollisionMaskOf(gameObject);
        }

        private void SyncTransform()
        {
            _collider.isTrigger = !type.collidable;
            if (deltaSize != 0) curSize += deltaSize * Time.deltaTime;

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
                transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * curSize, curSize, curSize);

            if (curSize <= 0) SelfDestruct();
        }

        private void SetupLayer()
        {
            if (type.hitAllies && type.hitEnemies)
                gameObject.layer = LayerMask.NameToLayer("HitBoth");
            else if (type.hitAllies)
                gameObject.layer = LayerMask.NameToLayer(owner.soul.alignment > 0 ? "HitAlly" : "HitEnemy");
            else
                gameObject.layer = LayerMask.NameToLayer(owner.soul.alignment > 0 ? "HitEnemy" : "HitAlly");
        }

        private void OnAnyCollide(Collider2D col)
        {
            var go = col.gameObject;
            if (go.layer == LayerMask.NameToLayer("Obstacle"))
                DecBounce();
            // _body.AddTorque(10f);
            if (IsBattleVesselGameObject(go))
            {
                var vessel = go.GetComponent<BattleVessel>();
                OnHit(vessel);
            }
        }

        private void DecBounce()
        {
            if (bounce < 0) return;
            if (bounce > 0)
            {
                bounce--;
                totalBounce++;
            }
            else
            {
                SelfDestruct();
            }
        }

        public void IncBounce(int k)
        {
            if (bounce >= 0) bounce += k;
        }

        public void IncPierce(int k)
        {
            pierce = Mathf.Max(0, pierce);
            pierce += k;
        }

        private bool IsBattleVesselGameObject(GameObject go)
        {
            return go.layer == LayerMask.NameToLayer("Allies") || go.layer == LayerMask.NameToLayer("Enemies");
        }

        private void OnHit(BattleVessel target)
        {
            ResolveHitEffect(target);
            pierce--;
            if (pierce == 0) SelfDestruct();
        }

        public void MoveUnder(BattleVessel target)
        {
            var transform1 = transform;
            transform1.parent = target.transform;
            transform1.localScale = Vector3.one / transform1.lossyScale.magnitude;
        }

        private void ResolveHitEffect(BattleVessel target)
        {
            // target.ApplyDamage(1, 0.5f, _body.velocity * 10);
            onHit.Invoke(target);
        }

        private void SelfDestruct()
        {
            DOTween.Kill(transform);
            beforeDestroy.Invoke();
            RogueManager.Instance.rogueUpdateEvent.RemoveListener(RogueUpdate);
            Destroy(gameObject);
        }
    }
}