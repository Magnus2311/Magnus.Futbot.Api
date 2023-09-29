namespace Magnus.Futtbot.Connections.Models
{
    public class ItemDataForMoving
    {
        public ItemDataForMoving(long id, string pile)
        {
            this.id = id;
            this.pile = pile;    
        }

        public long id { get; }

        public string pile { get; }
    }
}
