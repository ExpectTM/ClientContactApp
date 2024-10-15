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

            return RedirectToAction("Index", "ClientContact");
        }

        private string GetClientCode()
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

    }
}
