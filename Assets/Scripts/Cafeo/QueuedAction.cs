using Cafeo.Castable;

namespace Cafeo
{
    public abstract record QueuedAction
    {
        public record UseItemOfType : QueuedAction
        {
            public UsableItem.ItemTag tag;

            public UseItemOfType(UsableItem.ItemTag tag)
            {
                this.tag = tag;
            }
        }
    }
}