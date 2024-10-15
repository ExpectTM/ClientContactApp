using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientContactApp.Models
{
    public class Contact
    {
        [Key]
        public Guid ContactId { get; set; }

        [Required(ErrorMessage = "Contact name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Contact name must be between 3 and 150 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Surname name must be between 3 and 150 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        #nullable enable
        [NotMapped]
        public ICollection<ClientContact>? ClientContacts { get; set; }
        #nullable disable
    }
}
