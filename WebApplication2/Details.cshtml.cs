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
    public class DetailsModel : PageModel
    {
        private readonly WebApplication2.Models.WebApplication2Context _context;

        public DetailsModel(WebApplication2.Models.WebApplication2Context context)
        {
            _context = context;
        }

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
    }
}
