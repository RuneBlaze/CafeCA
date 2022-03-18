using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cafeo.Entities
{
    public class Collectable : MonoBehaviour
    {
        private Rigidbody2D body;
        private Collider2D collider;
        public SizeScale sizeScale = SizeScale.Small;
        
        public enum SizeScale
        {
            Small,
            Medium,
            Large
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Ally"))
            {
                OnCollect(Scene.GetVesselFromGameObject(col.gameObject));
                Destroy(gameObject);
            }
        }

        public virtual void Start()
        {
            body = GetComponent<Rigidbody2D>();
            collider = GetComponent<Collider2D>();
            body.drag = 2;
            body.mass = 5 + 10 * (int) sizeScale;
            transform.localScale = 
                sizeScale == SizeScale.Small ? Vector3.one * 0.27f : Vector3.one * (0.34f + 0.33f * (int) sizeScale);
        }

        public void AfterDropped()
        {
            body.AddForce(Random.insideUnitCircle * Random.Range(0, 2) * 10f);
        }

        protected RogueManager Scene => RogueManager.Instance;

        protected virtual void OnCollect(BattleVessel collector)
        {
            
        }
    }
}