using System;
using Cafeo.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Cafeo.Entities
{
    public class Collectable : MonoBehaviour
    {
        private Rigidbody2D body;
        private Collider2D collider;
        public SizeScale sizeScale = SizeScale.Small;
        private IDroppable load;

        private SpriteRenderer sprite;
        // public CollectableLoad load;
        
        public enum SizeScale
        {
            Small,
            Medium,
            Large
        }

        public void LateInit(IDroppable droppable)
        {
            load = droppable;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Ally"))
            {
                if (load != null)
                {
                    OnCollect(Scene.GetVesselFromGameObject(col.gameObject));
                    Destroy(gameObject);
                }
            }
        }

        public virtual void Start()
        {
            // Assert.IsNotNull(load);
            body = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
            sprite = GetComponent<SpriteRenderer>();
            sprite.sprite = load.Icon;
            if (load != null)
            {
                sizeScale = load.SizeScale;
            }
            body.drag = 2;
            body.mass = 5 + 10 * (int) sizeScale;
            transform.localScale = 
                sizeScale == SizeScale.Small ? Vector3.one * 0.27f : Vector3.one * (0.34f + 0.33f * (int) sizeScale);
            AfterDropped();
        }

        public void AfterDropped()
        {
            body.AddForce(Random.insideUnitCircle * Random.Range(0, 2) * 10f);
        }

        protected RogueManager Scene => RogueManager.Instance;

        protected virtual void OnCollect(BattleVessel collector)
        {
            load.OnPickedUp(collector);
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