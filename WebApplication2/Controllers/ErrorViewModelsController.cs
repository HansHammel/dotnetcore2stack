using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Produces("application/json")]
    [Route("api/ErrorViewModels")]
    public class ErrorViewModelsController : Controller
    {
        private readonly WebApplication2Context _context;

        public ErrorViewModelsController(WebApplication2Context context)
        {
            _context = context;
        }

        // GET: api/ErrorViewModels
        [HttpGet]
        public IEnumerable<ErrorViewModel> GetErrorViewModel()
        {
            return _context.ErrorViewModel;
        }

        // GET: api/ErrorViewModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetErrorViewModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var errorViewModel = await _context.ErrorViewModel.SingleOrDefaultAsync(m => m.RequestId == id);

            if (errorViewModel == null)
            {
                return NotFound();
            }

            return Ok(errorViewModel);
        }

        // PUT: api/ErrorViewModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutErrorViewModel([FromRoute] string id, [FromBody] ErrorViewModel errorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != errorViewModel.RequestId)
            {
                return BadRequest();
            }

            _context.Entry(errorViewModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ErrorViewModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ErrorViewModels
        [HttpPost]
        public async Task<IActionResult> PostErrorViewModel([FromBody] ErrorViewModel errorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ErrorViewModel.Add(errorViewModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetErrorViewModel", new { id = errorViewModel.RequestId }, errorViewModel);
        }

        // DELETE: api/ErrorViewModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteErrorViewModel([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var errorViewModel = await _context.ErrorViewModel.SingleOrDefaultAsync(m => m.RequestId == id);
            if (errorViewModel == null)
            {
                return NotFound();
            }

            _context.ErrorViewModel.Remove(errorViewModel);
            await _context.SaveChangesAsync();

            return Ok(errorViewModel);
        }

        private bool ErrorViewModelExists(string id)
        {
            return _context.ErrorViewModel.Any(e => e.RequestId == id);
        }
    }
}