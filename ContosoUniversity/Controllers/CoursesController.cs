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
    public class CoursesController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public CoursesController(ContosouniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourseAsync()
        {
            return await _context.Course.Where(x => !x.IsDeleted).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourseAsync(int id)
        {
            var course = await _context.Course.FindAsync(id);

            if (course == null || course.IsDeleted)
            {
                return NotFound();
            }

            return course;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourseAsync(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            course.DateModified = DateTime.Now;
            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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
        public async Task<ActionResult<Course>> PostCourseAsync(Course course)
        {
            await _context.Course.AddAsync(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Course>> DeleteCourseAsync(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.IsDeleted = true;
            await _context.SaveChangesAsync();

            return course;
        }

        [HttpGet("Students")]
        public async Task<ActionResult<IEnumerable<VwCourseStudents>>> GetCourseStudentsAsync()
        {
            return await _context.VwCourseStudents.ToListAsync();
        }

        [HttpGet("StudentCount")]
        public async Task<ActionResult<IEnumerable<VwCourseStudentCount>>> GetCourseStudentCountAsync()
        {
            return await _context.VwCourseStudentCount.ToListAsync();
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
