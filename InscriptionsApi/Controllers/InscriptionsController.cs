using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InscriptionsApiLocal.Models;

namespace InscriptionsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscriptionsController : ControllerBase
    {
        private readonly InscriptionsUniversityContext _context;

        public InscriptionsController(InscriptionsUniversityContext context)
        {
            _context = context;
        }

        // GET: api/Inscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inscription>>> GetInscriptions()
        {
          if (_context.Inscriptions == null)
          {
              return NotFound();
          }
            return await _context.Inscriptions.ToListAsync();
        }

        // GET: api/Inscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inscription>> GetInscription(int id)
        {
          if (_context.Inscriptions == null)
          {
              return NotFound();
          }
            var inscription = await _context.Inscriptions.FindAsync(id);

            if (inscription == null)
            {
                return NotFound();
            }

            return inscription;
        }

        // PUT: api/Inscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInscription(int id, Inscription inscription)
        {
            if (id != inscription.IncriptionId)
            {
                return BadRequest();
            }

            _context.Entry(inscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InscriptionExists(id))
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

        // POST: api/Inscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inscription>> PostInscription(Inscription inscription)
        {
          if (_context.Inscriptions == null)
          {
              return Problem("Entity set 'InscriptionsUniversityContext.Inscriptions'  is null.");
          }
            _context.Inscriptions.Add(inscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInscription", new { id = inscription.IncriptionId }, inscription);
        }

        // DELETE: api/Inscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInscription(int id)
        {
            if (_context.Inscriptions == null)
            {
                return NotFound();
            }
            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
            {
                return NotFound();
            }

            _context.Inscriptions.Remove(inscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InscriptionExists(int id)
        {
            return (_context.Inscriptions?.Any(e => e.IncriptionId == id)).GetValueOrDefault();
        }
    }
}
