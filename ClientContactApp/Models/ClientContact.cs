namespace ClientContactApp.Models
{
    public class ClientContact
    {
        public Guid ClientId { get; set; } 
        public Client Client { get; set; }

        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }


    }
}
