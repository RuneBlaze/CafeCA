using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Cafeo.Aimer
{
    public abstract class GenericAimer<T> : MonoBehaviour
    {
        private SpriteRenderer sprite;
        public abstract T Item { get; set; }
        [SerializeField] protected bool hidden;
        public BehaviorTree BehaviorTree { get; private set; }
        public virtual void Setup()
        {
            BehaviorTree = GetComponent<BehaviorTree>();
            sprite = GetComponent<SpriteRenderer>();
        }

        public GameObject TargetObject => (GameObject)BehaviorTree.GetVariable("TargetObject").GetValue();

        public virtual void SetupTargetTag(string targetTag)
        {
            BehaviorTree.SetVariableValue("TargetTag", targetTag);
            BehaviorTree.SetVariableValue("IgnoreTag", targetTag == "Player" ? "Enemies" : "Allies");
        }

        public void Hide()
        {
            hidden = true;
            if (sprite == null) return;
            sprite.enabled = false;
        }

        public void Show()
        {
            hidden = false;
            if (sprite == null) return;
            sprite.enabled = true;
        }
    }
}