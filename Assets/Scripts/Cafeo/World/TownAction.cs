namespace Cafeo.World
{
    public class TownAction
    {
        public int duration; // in turn units: 1 turn = 5 minutes
        public TownVessel owner;

        public TownAction(TownVessel owner, int duration)
        {
            this.owner = owner;
            this.duration = duration;
        }

        public virtual bool Disappearing => false;

        public virtual void OnUse()
        {
        }

        public virtual void OnEnd()
        {
        }

        public class Travel : TownAction
        {
            public TownNode destination;

            public Travel(TownVessel owner, int duration, TownNode destination) : base(owner, duration)
            {
                this.destination = destination;
            }

            public override void OnEnd()
            {
                base.OnEnd();
                owner.ReachDestination(destination);
            }
        }
    }
}