using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InscriptionsApiLocal.Models;
using InscriptionsApi.Controllers.DTO;
using System.Globalization;

namespace InscriptionsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscriptionsController : ControllerBase
    {
        private int tamInscriptions = 10;
        private readonly InscriptionsUniversityContext _context;

        public InscriptionsController(InscriptionsUniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inscription>>> GetInscriptions()
        {
            if (_context.Inscriptions == null)
            {
                return NotFound();
            }
            return await _context.Inscriptions.ToListAsync();
        }

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

        /* [HttpPut("{id}")]
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
                }*/
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, InscriptionData inscriptionData )
        {
            int studentId = inscriptionData.StudentId;
            string subjectName = inscriptionData.SubjectName;

            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectName == subjectName);
            if (subject == null)
            {
                return NotFound("Subject not found");
            }

            inscription.StudentId = studentId;
            inscription.SubjectId = subject.SubjectId;
            inscription.IncriptionDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InscriptionExistsTWO(id))
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

        private bool InscriptionExistsTWO(int id)
        {
            return _context.Inscriptions.Any(i => i.IncriptionId == id);
        }

        /*  [HttpPost]
          public async Task<ActionResult<Inscription>> PostInscription(Inscription inscription)
          {
              if (_context.Inscriptions == null)
              {
                  return Problem("Entity set 'InscriptionsUniversityContext.Inscriptions'  is null.");
              }
              _context.Inscriptions.Add(inscription);
              await _context.SaveChangesAsync();

              return CreatedAtAction("GetInscription", new { id = inscription.IncriptionId }, inscription);
          }*/
        [HttpPost]
        public async Task<ActionResult<InscriptionPostDTO>> PostInscription(InscriptionData inscriptionDta)
        {
            int studentId = inscriptionDta.StudentId;
            string subjectName = inscriptionDta.SubjectName;
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectName == subjectName);
            if (subject == null)
            {
                return NotFound("Subject not found");
            }

            var inscription = new InscriptionPostDTO()
            {
                StudentId = studentId,
                SubjectId = subject.SubjectId,
                IncriptionDate = DateTime.Now
            };

            if (_context.Inscriptions == null)
            {
                return Problem("Entity set 'InscriptionsUniversityContext.Inscriptions' is null.");
            }

            _context.Inscriptions.Add(new Inscription()
            {
                StudentId = inscription.StudentId,
                SubjectId = inscription.SubjectId,
                IncriptionDate = inscription.IncriptionDate
            });

            await _context.SaveChangesAsync();

            inscription.IncriptionId = inscription.IncriptionId;

            return CreatedAtAction("GetInscription", new { id = inscription.IncriptionId }, inscription);
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<InscriptionWithNames>>> GetInscriptionsWithNames(int pageNumber = 1, int pageSize = 10, string sortOrder = "", string sortBy = "", string searchString = "")
        {
            
        var studentsQuery = _context.Inscriptions.AsQueryable();
            var inscriptionForStudents = _context.Inscriptions.OrderBy(p => p.Student.StudentName).ToList();
            tamInscriptions = await studentsQuery.CountAsync();
            HttpContext.Response.Headers.Add("tamanio-inscriptions", tamInscriptions.ToString());
            var inscriptions = await _context.Inscriptions
                .Include(i => i.Student)
                .Include(i => i.Subject)
                .ToListAsync();
            // var pedidos = _context.Inscriptions.OrderBy(p => p.Student.StudentName).ToList();

            var inscriptionsfilterForSearch = inscriptionForStudents;
            //var aux = pedidos

            switch (sortBy)
            {
                case "InscriptionsId":
                    if (sortOrder == "asc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                            _context.Inscriptions.Where(s => s.IncriptionId.ToString().StartsWith(searchString)).OrderBy(p => p.IncriptionId).ToList() :
                            _context.Inscriptions.OrderBy(p => p.IncriptionId).ToList();

                    }
                    else if (sortOrder == "desc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                             _context.Inscriptions.Where(s => s.IncriptionId.ToString().StartsWith(searchString)).OrderByDescending(p => p.IncriptionId).ToList() :
                             _context.Inscriptions.OrderByDescending(p => p.IncriptionId).ToList();
                    }

                    break;
                case "studentName":
                    if (sortOrder == "asc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                            _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).OrderBy(p => p.Student.StudentName).ToList() :
                            _context.Inscriptions.OrderBy(p => p.Student.StudentName).ToList();

                    }
                    else if (sortOrder == "desc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                             _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).OrderByDescending(p => p.Student.StudentName).ToList() :
                             _context.Inscriptions.OrderByDescending(p => p.Student.StudentName).ToList();
                    }

                    break;
                case "subjectName":
                    if (sortOrder == "asc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                            _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).OrderBy(p => p.Subject.SubjectName).ToList() :
                            _context.Inscriptions.OrderBy(p => p.Subject.SubjectName).ToList();
                        
                    }
                    else if (sortOrder == "desc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                            _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).OrderByDescending(p => p.Subject.SubjectName).ToList() :
                            _context.Inscriptions.OrderByDescending(p => p.Subject.SubjectName).ToList();
                       
                    }
                    break;
                case "dateInscription":
                    if (sortOrder == "asc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                            _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).OrderBy(p => p.IncriptionDate).ToList() :
                            _context.Inscriptions.OrderBy(p => p.IncriptionDate).ToList();

                      //  inscriptionsfilterForSearch = _context.Inscriptions.OrderBy(p => p.IncriptionDate).ToList();
                    }
                    else if (sortOrder == "desc")
                    {
                        inscriptionsfilterForSearch = !string.IsNullOrEmpty(searchString) ?
                            _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).OrderByDescending(p => p.IncriptionDate).ToList() :
                            _context.Inscriptions.OrderByDescending(p => p.IncriptionDate).ToList();
                       // inscriptionsfilterForSearch = _context.Inscriptions.OrderByDescending(p => p.IncriptionDate).ToList();
                    }
                    break;
                default:
                    inscriptionsfilterForSearch = _context.Inscriptions.OrderBy(p => p.IncriptionId).ToList();

                    break;
            }
            inscriptionForStudents = inscriptionsfilterForSearch;

            if (!string.IsNullOrEmpty(searchString))
            {
                inscriptionForStudents = _context.Inscriptions.Where(s => s.Student.StudentName.StartsWith(searchString)).ToList();
            }

            var inscriptionsWithNames = inscriptionsfilterForSearch.Select(i => new InscriptionWithNames
            {
                IncriptionId = i.IncriptionId,
                StudentName = i.Student.StudentName,
                SubjectName = i.Subject.SubjectName,
                IncriptionDate = i.IncriptionDate
            }).ToList();

            var student = inscriptionsWithNames
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToList();

            return student;
        }


       

        [HttpGet("details/{id}")]
        public async Task<ActionResult<InscriptionWithNames>> GetInscriptionWithNames(int id)
        {
            var inscription = await _context.Inscriptions
                .Include(i => i.Student)
                .Include(i => i.Subject)
                .FirstOrDefaultAsync(i => i.IncriptionId == id);

            if (inscription == null)
            {
                return NotFound();
            }

            var inscriptionWithNames = new InscriptionWithNames
            {
                IncriptionId = inscription.IncriptionId,
                StudentName = inscription.Student.StudentName,
                SubjectName = inscription.Subject.SubjectName,
                IncriptionDate = inscription.IncriptionDate
            };

            return inscriptionWithNames;
        }



        




        private bool InscriptionExists(int id)
        {
            return (_context.Inscriptions?.Any(e => e.IncriptionId == id)).GetValueOrDefault();
        }

    }
}
