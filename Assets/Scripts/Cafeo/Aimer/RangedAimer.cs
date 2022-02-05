using System;
using BehaviorDesigner.Runtime;
using Cafeo.Castable;

namespace Cafeo.Aimer
{
    public class RangedAimer : GenericAimer<RangedItem>
    {
        private BehaviorTree _behaviorTree;
        public override RangedItem Item { get; set; }
        
        public void Setup()
        {
            _behaviorTree = GetComponent<BehaviorTree>();
        }

        public void SetupTargetTag(string targetTag)
        {
            var genVar = new SharedGenericVariable();
            _behaviorTree.SetVariableValue("TargetTag", targetTag);
            _behaviorTree.SetVariableValue("IgnoreTag", targetTag == "Player" ? "Enemies" : "Allies");
        }
    }
}