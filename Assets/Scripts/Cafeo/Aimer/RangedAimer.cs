using Cafeo.Castable;

namespace Cafeo.Aimer
{
    public class RangedAimer : GenericAimer<RangedItem>
    {
        public override RangedItem Item { get; set; }

        public override void ManualAim()
        {
            base.ManualAim();
        }
    }
}