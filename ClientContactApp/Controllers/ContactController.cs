using ClientContactApp.Data;
using ClientContactApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientContactApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult OnCreateContact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnCreateContact(Contact contactEntry)
        {

            bool doesEmailExist = _context.Contacts.Any(u => u.Email == contactEntry.Email);

            if (doesEmailExist) 
            {
                TempData["error"] = "Email already exist";

                return View();
            }

            Contact contact = new Contact()
            {
                ContactId = Guid.NewGuid(),
                Name = contactEntry.Name,
                Surname = contactEntry.Surname,
                Email = contactEntry.Email,
            };

            _context.Contacts.Add(contact);

            await _context.SaveChangesAsync();

            TempData["success"] = "Contact created successfully";

            return RedirectToAction("Index", "ClientContact");
        }

    }
}
