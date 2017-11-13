using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2
{
    public class DeleteModel : PageModel
    {
        private readonly WebApplication2.Models.WebApplication2Context _context;

        public DeleteModel(WebApplication2.Models.WebApplication2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public ErrorViewModel ErrorViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ErrorViewModel = await _context.ErrorViewModel.SingleOrDefaultAsync(m => m.RequestId == id);

            if (ErrorViewModel == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ErrorViewModel = await _context.ErrorViewModel.FindAsync(id);

            if (ErrorViewModel != null)
            {
                _context.ErrorViewModel.Remove(ErrorViewModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
