using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Cafeo.Aimer
{
    public abstract class GenericAimer<T> : MonoBehaviour
    {
        public abstract T Item { get; set; }
        public BehaviorTree BehaviorTree { get; private set; }
        public virtual void Setup()
        {
            BehaviorTree = GetComponent<BehaviorTree>();
        }

        public GameObject TargetObject => (GameObject)BehaviorTree.GetVariable("TargetObject").GetValue();

        public virtual void SetupTargetTag(string targetTag)
        {
            BehaviorTree.SetVariableValue("TargetTag", targetTag);
            BehaviorTree.SetVariableValue("IgnoreTag", targetTag == "Player" ? "Enemies" : "Allies");
        }
    }
}