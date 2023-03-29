using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InscriptionsApiLocal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;

namespace ApiMaterias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly InscriptionsUniversityContext _context;
        private int tamSubjects = 0;

        public SubjectsController(InscriptionsUniversityContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }

            return await _context.Subjects.ToListAsync();
        }

        [HttpGet("withSorts")]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjectsWithFilters(string sortBy = "Id", string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }

            var subjects = _context.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        subjects = (sortOrder == "desc") ? subjects.OrderByDescending(s => s.SubjectName) : subjects.OrderBy(s => s.SubjectName);
                        break;
                    case "id":
                        subjects = (sortOrder == "desc") ? subjects.OrderByDescending(s => s.SubjectId) : subjects.OrderBy(s => s.SubjectId);
                        break;
                    case "status":
                        subjects = (sortOrder == "desc") ? subjects.OrderByDescending(s => s.SubjectStatus) : subjects.OrderBy(s => s.SubjectStatus);
                        break;
                    case "capacity":
                        subjects = (sortOrder == "desc") ? subjects.OrderByDescending(s => s.SubjectCapacity) : subjects.OrderBy(s => s.SubjectCapacity);
                        break;
                    default:
                        subjects = subjects.OrderBy(s => s.SubjectId);
                        break;
                }
            }

            var totalCount = await subjects.CountAsync();
            this.tamSubjects = totalCount;

            HttpContext.Response.Headers.Add("tamanio-subjects", tamSubjects.ToString());

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var items = await subjects.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
          if (_context.Subjects == null)
          {
              return NotFound();
          }
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return subject;
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return BadRequest();
            }

            _context.Entry(subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
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

        
        [HttpPost]
        public async Task<ActionResult<Subject>> PostSubject(Subject subject)
        {
          if (_context.Subjects == null)
          {
              return Problem("Entity set 'InscriptionsUniversityContext.Subjects'  is null.");
          }
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubject", new { id = subject.SubjectId }, subject);
        }


        [HttpPatch("{id}/state")]
        public async Task<IActionResult> ChangeStatusSubject(int id, int state)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            subject.SubjectStatus = state;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
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

        private bool SubjectExists(int id)
        {
            return (_context.Subjects?.Any(e => e.SubjectId == id)).GetValueOrDefault();
        }
    }
}
