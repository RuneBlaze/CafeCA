using System;
using BehaviorDesigner.Runtime;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cafeo.Aimer
{
    public abstract class GenericAimer<T> : MonoBehaviour
    {
        private SpriteRenderer sprite;
        public abstract T Item { get; set; }
        [SerializeField] protected bool hidden;

        public bool autoAim;
        protected BattleVessel battleVessel;
        public BehaviorTree BehaviorTree { get; private set; }
        public virtual void Setup()
        {
            autoAim = true;
            BehaviorTree = GetComponent<BehaviorTree>();
            sprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            battleVessel = transform.parent.GetComponent<BattleVessel>();
            Assert.IsNotNull(battleVessel);
        }

        public GameObject TargetObject => (GameObject)BehaviorTree.GetVariable("TargetObject").GetValue();

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

        public virtual void Update()
        {
            BehaviorTree.enabled = autoAim;
            if (!autoAim && Item != null)
            {
                ManualAim();
            }
        }

        public virtual void Refresh()
        {
            
        }

        public virtual void ManualAim()
        {
            if (battleVessel.IsPlayer)
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