﻿using Cafeo.Data;
using UnityEngine;

namespace Cafeo.Entities
{
    public class Collectable : MonoBehaviour
    {
        public enum SizeScale
        {
            Small,
            Medium,
            Large
        }

        public SizeScale sizeScale = SizeScale.Small;

        public int price;
        private Rigidbody2D body;
        private Collider2D collider;
        private Color disabledColor;
        private IDroppable load;
        private float pickupWait = 1.0f; // time until this item can be picked up
        private SpriteRenderer sprite;

        protected RogueManager Scene => RogueManager.Instance;

        public virtual void Start()
        {
            collider = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
            disabledColor = new Color(1, 1, 1, 0.5f);
            RogueManager.Instance.rogueUpdateEvent.AddListener(OnRogueUpdate);
            sprite.sprite = load.Icon;
            if (load != null) sizeScale = load.SizeScale;
            body.drag = 0.8f;
            body.mass = 3 + 3 * (int)sizeScale;
            transform.localScale =
                sizeScale == SizeScale.Small ? Vector3.one * 0.27f : Vector3.one * (0.34f + 0.33f * (int)sizeScale);
            AfterDropped();
        }

        private void Update()
        {
            sprite.color = pickupWait > 0 ? disabledColor : Color.white;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (pickupWait > 0) return;
            if (col.collider.CompareTag("Ally"))
                if (load != null)
                {
                    if (price > 0)
                    {
                        if (col.gameObject == Scene.player.gameObject)
                        {
                            // only the player can buy the items
                            var party = AllyParty.Instance;
                            if (party.Gold >= price)
                            {
                                party.LoseGold(price);
                                OnCollect(Scene.GetVesselFromGameObject(col.gameObject));
                                Destroy(gameObject);
                            }
                            // .LoseGold();
                        }
                    }
                    else
                    {
                        OnCollect(Scene.GetVesselFromGameObject(col.gameObject));
                        Destroy(gameObject);
                    }
                }
        }

        public void LateInit(IDroppable droppable)
        {
            load = droppable;
            body = GetComponent<Rigidbody2D>();
        }

        public void AttachPrice(int coins)
        {
            price = coins;
            var tag = Scene.AttachLabel(transform);
            tag.text = $"$ {price}";
        }

        public void AfterDropped()
        {
            body.AddForce(Random.insideUnitCircle * Random.Range(0, 2) * 10f);
        }

        protected virtual void OnCollect(BattleVessel collector)
        {
            if (load.CanBePickedUp(collector)) load.OnPickedUp(collector);
        }

        private void OnRogueUpdate()
        {
            pickupWait -= Time.deltaTime;
            pickupWait = Mathf.Max(0, pickupWait);
        }

        public void BeThrown(Vector2 dir)
        {
            body.AddForce(dir * 150f);
        }

        // public abstract class CollectableLoad
        // {
        //     
        // }
        //
        // public class TreasureLoad : CollectableLoad
        // {
        //     public Treasure treasure;
        // }
        //
        // public class BasicsLoad : CollectableLoad
        // {
        //     public int gold;
        //     public int torches;
        //     public int keys;
        // }
    }
}