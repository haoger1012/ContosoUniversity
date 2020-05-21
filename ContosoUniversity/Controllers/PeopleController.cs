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
    public class PeopleController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public PeopleController(ContosouniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonAsync()
        {
            return await _context.Person.Where(x => !x.IsDeleted).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPersonAsync(int id)
        {
            var person = await _context.Person.FindAsync(id);

            if (person == null || person.IsDeleted)
            {
                return NotFound();
            }

            return person;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonAsync(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            person.DateModified = DateTime.Now;
            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
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
        public async Task<ActionResult<Person>> PostPersonAsync(Person person)
        {
            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> DeletePersonAsync(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            person.IsDeleted = true;
            await _context.SaveChangesAsync();

            return person;
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.Id == id);
        }
    }
}
