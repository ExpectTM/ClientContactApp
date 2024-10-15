using System.ComponentModel.DataAnnotations;

namespace ClientContactApp.Models
{
    public class Client
    {
        [Key]
        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string ClientCode { get; set; }

        #nullable enable
        public ICollection<ClientContact>? ClientContacts { get; set; }
        #nullable disable
    }
}
