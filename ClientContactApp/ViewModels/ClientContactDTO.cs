using ClientContactApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ClientContactApp.ViewModels
{
    public class ClientContactDTO
    {
        public List<ClientDTO> Clients { get; set; }
        public List<ContactDTO> Contacts { get; set; }

        public class ClientDTO
        {
            public Guid ClientId { get; set; }
            public string ClientName { get; set; }
            public string ClientCode { get; set; }
        }

        public class ContactDTO
        {
            public Guid ContactId { get; set; }
            public string Email { get; set; }
            public string Surname { get; set; }
            public string ContactName { get; set; }
        }
    }
}
