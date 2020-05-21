using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using Microsoft.Data.SqlClient;

namespace ContosoUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentAsync()
        {
            return await _context.Department.Where(x => !x.IsDeleted).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartmentAsync(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null || department.IsDeleted)
            {
                return NotFound();
            }

            return department;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartmentAsync(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            //string sql = "EXEC [dbo].[Department_Update] @DepartmentID, @Name, @Budget, @StartDate, @InstructorID, @RowVersion_Original";
            //await _context.Database.ExecuteSqlRawAsync(sql,
            //    new SqlParameter("@DepartmentID", department.DepartmentId),
            //    new SqlParameter("@Name", department.Name),
            //    new SqlParameter("@Budget", department.Budget),
            //    new SqlParameter("@StartDate", department.StartDate),
            //    new SqlParameter("@InstructorID", department.InstructorId),
            //    new SqlParameter("@RowVersion_Original", department.RowVersion));

            department.DateModified = DateTime.Now;
            _context.Entry(department).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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
        public async Task<ActionResult<Department>> PostDepartmentAsync(Department department)
        {
            string sql = "EXEC [dbo].[Department_Insert] @Name, @Budget, @StartDate, @InstructorID";
            await _context.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@Name", department.Name),
                new SqlParameter("@Budget", department.Budget),
                new SqlParameter("@StartDate", department.StartDate),
                new SqlParameter("@InstructorID", department.InstructorId));

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartmentAsync(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            //string sql = "EXEC [dbo].[Department_Delete] @DepartmentID, @RowVersion_Original";
            //await _context.Database.ExecuteSqlRawAsync(sql,
            //    new SqlParameter("@DepartmentID", department.DepartmentId),
            //    new SqlParameter("@RowVersion_Original", department.RowVersion));

            department.IsDeleted = true;
            await _context.SaveChangesAsync();

            return department;
        }

        [HttpGet("CourseCount")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetDepartmentCourseCountAsync()
        {
            string sql = "SELECT [DepartmentID],[Name],[CourseCount] FROM [dbo].[vwDepartmentCourseCount]";
            return await _context.VwDepartmentCourseCount.FromSqlRaw(sql).ToListAsync();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
