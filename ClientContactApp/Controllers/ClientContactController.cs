using ClientContactApp.Data;
using ClientContactApp.Models;
using ClientContactApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClientContactApp.Controllers
{
    public class ClientContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var contacts = _context.Contacts.ToList();

            var clients = _context.Clients.ToList();

            ClientContactDTO clientContactDTO = new ClientContactDTO
            {
                Contacts = new List<ClientContactDTO.ContactDTO>(),

                Clients = new List<ClientContactDTO.ClientDTO>()
            };

            
            Dictionary<Guid, int> clientContactCounts = new Dictionary<Guid, int>();

            foreach (var client in clients)
            {
                int linkContacts = await GetLinkedContactCount(client.ClientId);

                clientContactCounts[client.ClientId] = linkContacts;
            }

            ViewData["ClientContactCounts"] = clientContactCounts;


            Dictionary<Guid, int> contactClientCounts = new Dictionary<Guid, int>();

            foreach (var contact in contacts)
            {
                int linkClients = await GetLinkedClientCount(contact.ContactId);

                contactClientCounts[contact.ContactId] = linkClients;
            }

            ViewData["ContactClientCounts"] = contactClientCounts;


            foreach (var contact in contacts)
            {
                var contactDTO = new ClientContactDTO.ContactDTO
                {
                    ContactId = contact.ContactId,
                    Email = contact.Email,
                    Surname = contact.Surname,
                    ContactName = contact.Name
                };

                clientContactDTO.Contacts.Add(contactDTO);
            }

            
            foreach (var client in clients)
            {
                var clientDTO = new ClientContactDTO.ClientDTO
                {
                    ClientId = client.ClientId,
                    ClientName = client.Name,
                    ClientCode = client.ClientCode
                };

                clientContactDTO.Clients.Add(clientDTO);
            }

            return View(clientContactDTO);
        }


        [HttpGet]
        public IActionResult LinkContactsToClient(Guid clientId)
        {
            var contactList = _context.Contacts.ToList();

            var clients = _context.Clients.Find(clientId);

            ViewData["clientId"] = clientId;

            ViewData["clientName"] = clients.Name;

            return View(contactList);
        }

        [HttpPost]
        public async Task<IActionResult> LinkContactsToClient(Guid clientId, List<Guid> contactIds)
        {
             var client = await _context.Clients.FindAsync(clientId);

            if (client == null)
            {
                TempData["error"] = "Client not found.";

                return RedirectToAction("Index");
            }

            
            var contacts = await _context.Contacts
                .Where(c => contactIds.Contains(c.ContactId))
                .ToListAsync();


            foreach (var contact in contacts)
            {
                var existingLink = await _context.ClientContacts
                    .FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contact.ContactId);


                if (existingLink == null)
                {
                    var clientContact = new ClientContact
                    {
                        ClientId = clientId,
                        ContactId = contact.ContactId
                    };
                    _context.ClientContacts.Add(clientContact);
                }
                else
                {
                    TempData["error"] = "Contact already linked";

                    return RedirectToAction("Index");
                }
            }

            await _context.SaveChangesAsync();

            TempData["success"] = "Contacts linked to client successfully.";

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult UnlinkContactsFromClient(Guid contactId)
        {
           
            var clientList = _context.Clients
                .Where(c => _context.ClientContacts.Any(cc => cc.ClientId == c.ClientId && cc.ContactId == contactId))
                .ToList();

            var contact = _context.Contacts.Find(contactId);

            if (contact == null)
            {
                TempData["error"] = "Contact not found.";

                return NotFound("Index");
            }

            ViewData["contactId"] = contactId;

            ViewData["FullName"] = $"{contact.Name} {contact.Surname}";

            return View(clientList);
        }


        [HttpPost]
        public async Task<IActionResult> UnlinkContactsFromClient(Guid contactId, List<Guid> clientIds)
        {
           
            var clientContacts = await _context.ClientContacts
                .Where(cc => cc.ContactId == contactId && clientIds.Contains(cc.ClientId))
                .ToListAsync();

            if (clientContacts == null || clientContacts.Count == 0)
            {
                TempData["error"] = "No linked contacts found to unlink.";

                return RedirectToAction("Index", "Contact");
            }


            _context.ClientContacts.RemoveRange(clientContacts);

            await _context.SaveChangesAsync();

            TempData["success"] = "Contacts unlinked from client successfully.";

            return RedirectToAction("Index");
        }


        private async Task<int> GetLinkedContactCount(Guid clientId)
        {
            var linkedContactCount = await _context.ClientContacts
                .Where(cc => cc.ClientId == clientId)
                .CountAsync();

            return linkedContactCount;
        }

        private async Task<int> GetLinkedClientCount(Guid contactId)
        {
            var linkedClientCount = await _context.ClientContacts
                .Where(cc => cc.ContactId == contactId)
                .CountAsync();

            return linkedClientCount;
        }

    }

}
