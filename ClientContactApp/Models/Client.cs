using System.ComponentModel.DataAnnotations;

namespace ClientContactApp.Models
{
    public class Client
    {
        [Key]
        public Guid ClientId { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Client name must be between 3 and 150 characters.")]
        public string Name { get; set; }

        public string ClientCode { get; set; }

        #nullable enable
        public ICollection<ClientContact>? ClientContacts { get; set; }
        #nullable disable
    }
}
