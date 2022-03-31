using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Advanced.API.Practice.Entities;
using DAL.DbContexts;
using Shared;

namespace Advanced.API.Practice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherContext _context;

        public TeachersController(TeacherContext context)
        {
            _context = context;
        }

        [HttpPost("list")]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers([FromBody] Paging page)
        {
            var list = await _context.Teachers.Include(z => z.Courses).Where(s => s.FirstName.Contains(page.SearchString ?? string.Empty)).DefaultIfEmpty().ToListAsync();

            if (page == null || page.Size == 0)
            {
                list.ToList();
            }
            else
            {
                list.Take(page.Size * page.PageNumber)
                    .Skip(page.Size * (page.PageNumber - 1));
            }
            
            Response.Headers.Add("X-Total-Count", list.Count.ToString());
            Response.Headers.Add("Accept", "application/json");

            return list;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(Guid id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(Guid id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
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

        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] string name)
        {
            try
            {
                var result = _context.Teachers.FirstOrDefault(n => n.Id == id);
                if (result != null)
                {
                    result.FirstName = name;
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacher", new { id = teacher.Id }, teacher);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<Teacher>> DeleteTeacher(Guid id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }

        private bool TeacherExists(Guid id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
