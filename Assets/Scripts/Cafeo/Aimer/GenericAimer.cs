using BehaviorDesigner.Runtime;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Aimer
{
    public abstract class GenericAimer<T> : MonoBehaviour
    {
        [SerializeField] protected bool hidden;

        public bool autoAim;
        public bool locked;
        protected BattleVessel battleVessel;
        private SpriteRenderer sprite;
        public abstract T Item { get; set; }
        public BehaviorTree BehaviorTree { get; private set; }

        public GameObject TargetObject => (GameObject)BehaviorTree.GetVariable("TargetObject").GetValue();

        private void Start()
        {
            battleVessel = transform.parent.GetComponent<BattleVessel>();
            Assert.IsNotNull(battleVessel);
        }

        public virtual void Update()
        {
            BehaviorTree.enabled = autoAim && !locked;
            if (!autoAim && Item != null) ManualAim();
        }

        public virtual void Setup()
        {
            autoAim = true;
            BehaviorTree = GetComponent<BehaviorTree>();
            sprite = GetComponent<SpriteRenderer>();
        }

        public virtual void SetupTargetTag(string targetTag)
        {
            BehaviorTree.SetVariableValue("TargetTag", targetTag);
            BehaviorTree.SetVariableValue("IgnoreTag", targetTag == "Ally" ? "Enemies" : "Allies");
        }

        public void Hide()
        {
            hidden = true;
            if (sprite == null) return;
            sprite.enabled = false;
        }

        public virtual void Refresh()
        {
        }

        public virtual void ManualAim()
        {
            if (battleVessel.IsPlayer && !locked)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                VectorUtils.RotateTowards(transform, mousePos, 16);
            }
        }

        public void Show()
        {
            hidden = false;
            if (sprite == null) return;
            sprite.enabled = true;
        }
    }
}