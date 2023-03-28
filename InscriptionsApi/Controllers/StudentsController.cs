﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InscriptionsApiLocal.Models;

namespace lab2_Distribuidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly InscriptionsUniversityContext _context;

        public StudentsController(InscriptionsUniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            if (_context.Students == null)
            {
                return NotFound();
            }
            return await _context.Students.ToListAsync();
        }

        [HttpGet("withSorts")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents(int pageNumber = 1, int pageSize = 10, string sortOrder = "", string sortBy = "", string searchString = "")
        {
            var studentsQuery = _context.Students.AsQueryable();

            studentsQuery = ApplySearchStringFilter(studentsQuery, searchString);
            studentsQuery = ApplySortOrderBy(studentsQuery, sortOrder, sortBy);

            var students = await studentsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return students;
        }


        private IQueryable<Student> ApplySearchStringFilter(IQueryable<Student> studentsQuery, string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                studentsQuery = studentsQuery.Where(s => s.StudentName.StartsWith(searchString));
            }

            return studentsQuery;
        }

        private IQueryable<Student> ApplySortOrderBy(IQueryable<Student> studentsQuery, string sortOrder, string sortBy)
        {
            switch (sortBy)
            {
                case "studentName":
                    studentsQuery = sortOrder switch
                    {
                        "0" => studentsQuery.OrderBy(s => s.StudentName),
                        "1" => studentsQuery.OrderByDescending(s => s.StudentName),
                        "" => studentsQuery.OrderBy(s => s.StudentName),
                        _ => throw new ArgumentException("sortOrder incorrecto, por favor ingrese para ascendente y 1 para descendente", nameof(sortOrder))
                    };
                    break;
                case "studentId":
                    studentsQuery = sortOrder switch
                    {
                        "0" => studentsQuery.OrderBy(s => s.StudentId),
                        "1" => studentsQuery.OrderByDescending(s => s.StudentId),
                        "" => studentsQuery.OrderBy(s => s.StudentId),
                        _ => throw new ArgumentException("sortOrder incorrecto, por favor ingrese para ascendente y 1 para descendente", nameof(sortOrder))
                    };
                    break;
                case "studentLn":
                    studentsQuery = sortOrder switch
                    {
                        "0" => studentsQuery.OrderBy(s => s.StudentLn),
                        "1" => studentsQuery.OrderByDescending(s => s.StudentLn),
                        "" => studentsQuery.OrderBy(s => s.StudentLn),
                        _ => throw new ArgumentException("sortOrder incorrecto, por favor ingrese para ascendente y 1 para descendente", nameof(sortOrder))
                    };
                    break;
                case "typeDocStudent":
                    studentsQuery = sortOrder switch
                    {
                        "0" => studentsQuery.OrderBy(s => s.TypeDocStudent),
                        "1" => studentsQuery.OrderByDescending(s => s.TypeDocStudent),
                        "" => studentsQuery.OrderBy(s => s.TypeDocStudent),
                        _ => throw new ArgumentException("sortOrder incorrecto, por favor ingrese para ascendente y 1 para descendente", nameof(sortOrder))
                    };
                    break;
                case "studentGenre":
                    studentsQuery = sortOrder switch
                    {
                        "0" => studentsQuery.OrderBy(s => s.StudentGenre),
                        "1" => studentsQuery.OrderByDescending(s => s.StudentGenre),
                        "" => studentsQuery.OrderBy(s => s.StudentGenre),
                        _ => throw new ArgumentException("sortOrder incorrecto, por favor ingrese para ascendente y 1 para descendente", nameof(sortOrder))
                    };
                    break;
                default:
                    studentsQuery = sortOrder switch
                    {
                        "0" => studentsQuery.OrderBy(s => s.StudentId),
                        "1" => studentsQuery.OrderByDescending(s => s.StudentId),
                        "" => studentsQuery.OrderBy(s => s.StudentId),
                        _ => throw new ArgumentException("sortOrder incorrecto, por favor ingrese para ascendente y 1 para descendente", nameof(sortOrder))
                    };
                    break;
            }

            return studentsQuery;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            if (_context.Students == null)
            {
                return NotFound();
            }
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'InscriptionsUniversityContext.Students'  is null.");
            }
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        [HttpPatch("{id}/state")]
        public async Task<IActionResult> ChangeStatusStudent(int id, int state)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            student.StudentStatus = state;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        private bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
