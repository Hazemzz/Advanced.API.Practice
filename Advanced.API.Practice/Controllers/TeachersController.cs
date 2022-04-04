using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Advanced.API.Practice.Entities;
using DAL.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Shared;

namespace Advanced.API.Practice.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TeacherContext _context;

        public TeachersController(IMemoryCache memoryCache, TeacherContext context)
        {
            _memoryCache = memoryCache;
            _context = context;
        }

        [HttpPost("list")]
        public async Task<List<Teacher>> GetTeachers([FromBody] Paging page)
        {
            var cacheKey = "teacherList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<Teacher> list))
            {
                 list = await _context.Teachers.Include(z => z.Courses).ToListAsync();

                if (page == null || page.Size == 0)
                {
                    list
                        .Where(s => s.FirstName.Contains(page.SearchString ?? string.Empty)).DefaultIfEmpty().ToList();
                }
                else
                {
                    list
                        .Where(s => s.FirstName.Contains(page.SearchString ?? string.Empty)).DefaultIfEmpty().Take(page.Size * page.PageNumber)
                        .Skip(page.Size * (page.PageNumber - 1));
                }
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, list, cacheExpiryOptions);
                Response.Headers.Add("Accept", "application/json");
            }

            return  list;
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
