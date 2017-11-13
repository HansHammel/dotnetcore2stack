using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2.Models;

namespace WebApplication2
{
    public class CreateModel : PageModel
    {
        private readonly WebApplication2.Models.WebApplication2Context _context;

        public CreateModel(WebApplication2.Models.WebApplication2Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ErrorViewModel ErrorViewModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ErrorViewModel.Add(ErrorViewModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}