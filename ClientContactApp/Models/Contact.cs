using System.ComponentModel.DataAnnotations;

namespace ClientContactApp.Models
{
    public class Contact
    {
        [Key]
        public Guid ContactId { get; set; } 
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        #nullable enable
        public ICollection<ClientContact>? ClientContacts { get; set; }
        #nullable disable
    }
}
