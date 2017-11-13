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
    public class IndexModel : PageModel
    {
        private readonly WebApplication2.Models.WebApplication2Context _context;

        public IndexModel(WebApplication2.Models.WebApplication2Context context)
        {
            _context = context;
        }

        public IList<ErrorViewModel> ErrorViewModel { get;set; }

        public async Task OnGetAsync()
        {
            ErrorViewModel = await _context.ErrorViewModel.ToListAsync();
        }
    }
}
