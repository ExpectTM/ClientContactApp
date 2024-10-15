using ClientContactApp.Data;
using ClientContactApp.Models;
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
                return NotFound("Client not found.");
            }

            
            var contacts = await _context.Contacts
                .Where(c => contactIds.Contains(c.ContactId))
                .ToListAsync();

            if (contacts.Count != contactIds.Count)
            {
                return BadRequest("Some of the contacts provided were not found.");
            }

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
            }

            await _context.SaveChangesAsync();

            TempData["success"] = "Contacts linked to client successfully.";

            return RedirectToAction("Index", "Client");
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
                return NotFound("Contact not found.");
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

            return RedirectToAction("Index", "Contact");
        }



    }
}
