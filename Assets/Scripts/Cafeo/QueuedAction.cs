using Cafeo.Castable;

namespace Cafeo
{
    public class QueuedAction
    {
        public static int nextId;
        public int id;

        protected QueuedAction()
        {
            SetId();
        }

        private void SetId()
        {
            id = nextId++;
        }

        public class UseItemOfType : QueuedAction
        {
            public UsableItem.ItemTag tag;

            public UseItemOfType(UsableItem.ItemTag tag)
            {
                this.tag = tag;
            }
        }
    }
}