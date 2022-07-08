using Cafeo.World;

namespace Cafeo.Messaging
{
    public class DialogMessage : AbstractMessage
    {
        public TownVessel from;
        public string message;

        public DialogMessage(TownVessel from, string message)
        {
            this.from = from;
            this.message = message;
        }
    }
}