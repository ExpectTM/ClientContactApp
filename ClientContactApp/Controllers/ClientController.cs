using ClientContactApp.Data;
using ClientContactApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientContactApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context) 
        { 
            _context = context; 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<Client> clients = await _context.Clients.ToListAsync();

            Dictionary<Guid, int> clientContactCounts = new Dictionary<Guid, int>();

            foreach (var client in clients)
            {
                int linkContacts = await GetLinkedContactCount(client.ClientId);
                clientContactCounts[client.ClientId] = linkContacts;
            }

            ViewData["ClientContactCounts"] = clientContactCounts;

            if (clients is null)
            {
                return RedirectToAction("No client(s) found");
            }

            return View(clients);
        }


        public async Task<int> GetLinkedContactCount(Guid clientId)
        {
            var linkedContactCount = await _context.ClientContacts
                .Where(cc => cc.ClientId == clientId)
                .CountAsync();

            return linkedContactCount;
        }


        [HttpGet]
        public IActionResult OnCreateClient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnCreateClient(Client clientEntry)
        {
            Client client = new Client()
            {
                ClientId = Guid.NewGuid(),
                Name = clientEntry.Name,
                ClientCode = GetClientCode(),
            };

                _context.Clients.Add(client);

                await _context.SaveChangesAsync();

            TempData["success"] = "Client created successfully";

            return RedirectToAction("Index");
        }

        public string GetClientCode()
        {
            
            var lastClient = _context.Clients
                .OrderByDescending(c => c.ClientCode)
                .FirstOrDefault();

            
            int numberSequence = 1;

            if (lastClient != null && int.TryParse(lastClient.ClientCode.Substring(3), out int lastNumber))
            {
                
                numberSequence = lastNumber + 1;
            }

            string newClientCode = $"BCA{numberSequence:D3}"; 

            return newClientCode;
        }

        [HttpGet]
        public async Task<IActionResult> ContactsByClient(Guid clientId)
        {
            var clientContacts = await _context.ClientContacts
                .Where(cc => cc.ClientId == clientId)
                .Include(cc => cc.Contact)
                .ToListAsync();

            return View(clientContacts);
        }
    }
}
