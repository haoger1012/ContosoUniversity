using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseInstructorsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public CourseInstructorsController(ContosouniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseInstructor>>> GetCourseInstructorAsync()
        {
            return await _context.CourseInstructor.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseInstructor>> GetCourseInstructorAsync(int id)
        {
            var courseInstructor = await _context.CourseInstructor.FindAsync(id);

            if (courseInstructor == null)
            {
                return NotFound();
            }

            return courseInstructor;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseInstructorAsync(int id, CourseInstructor courseInstructor)
        {
            if (id != courseInstructor.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(courseInstructor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseInstructorExists(id))
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
        public async Task<ActionResult<CourseInstructor>> PostCourseInstructorAsync(CourseInstructor courseInstructor)
        {
            await _context.CourseInstructor.AddAsync(courseInstructor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseInstructorExists(courseInstructor.CourseId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourseInstructor", new { id = courseInstructor.CourseId }, courseInstructor);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseInstructor>> DeleteCourseInstructorAsync(int id)
        {
            var courseInstructor = await _context.CourseInstructor.FindAsync(id);
            if (courseInstructor == null)
            {
                return NotFound();
            }

            _context.CourseInstructor.Remove(courseInstructor);
            await _context.SaveChangesAsync();

            return courseInstructor;
        }

        private bool CourseInstructorExists(int id)
        {
            return _context.CourseInstructor.Any(e => e.CourseId == id);
        }
    }
}
