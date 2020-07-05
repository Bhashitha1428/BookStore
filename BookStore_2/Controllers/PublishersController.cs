using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore_2.Models;

namespace BookStore_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly BookStoresDBContext _context;

        public PublishersController(BookStoresDBContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublisher()
        {
            return await _context.Publisher.ToListAsync();
        }


        //***********************************************//

        // GET: api/Publishers/GetPublisherDetails/5
        [HttpGet("GetPublisherDetails/{id}")]
        public async Task<ActionResult<Publisher>> GetPublisherDetals(int id)
        {
            //***Eager Loading
            //var publisher = _context.Publisher
            //                                  .Include(pub => pub.Book)
            //                                       .ThenInclude(book => book.Sale)
            //                                  .Include(pub => pub.User)
            //                                        .ThenInclude(user => user.Job)
            //                                  .Where(pub => pub.PubId == id)
            //                                  .FirstOrDefault();


            //**Explicit Loading
            var publisher = await _context.Publisher.SingleAsync(pub => pub.PubId == id);
            _context.Entry(publisher)
                .Collection(pub => pub.User)
                .Query()
                .Where(user=>user.EmailAddress.Contains("karin"))
                .Load();
            _context.Entry(publisher)
                     .Collection(pub => pub.Book)
                     .Query()
                     .Include(book => book.Sale)
                     .Load();

        


            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // GET: api/Publishers/PostPublisherDetals
        [HttpGet("PostPublisherDetals")]
        public async Task<ActionResult<Publisher>> PostPublisherDetals()
        {
            var publisher_1 = new Publisher();
            publisher_1.PublisherName = "Bhashitha";
            publisher_1.City = "Elpitiya";
            publisher_1.State = "G";
            publisher_1.Country = "Srilanka";

            Book b1 = new Book();
            b1.Title = "Good night moon 1";
            b1.PublishedDate = DateTime.Now;

            Book b2 = new Book();
            b2.Title = "Good night moon 2";
            b2.PublishedDate = DateTime.Now;


            Sale sale1 = new Sale();
            sale1.Quantity = 2;
            sale1.StoreId = "8042";
            sale1.OrderNum = "XYZ";
            sale1.PayTerms = "Net 30";
            sale1.OrderDate = DateTime.Now;

            Sale sale2 = new Sale();
            sale2.Quantity = 2;
            sale2.StoreId = "7131";
            sale2.OrderNum = "QA879.1";
            sale2.PayTerms = "Net 20";
            sale2.OrderDate = DateTime.Now;

            b1.Sale.Add(sale1);
            b2.Sale.Add(sale2);

            publisher_1.Book.Add(b1);
            publisher_1.Book.Add(b2);

            _context.Publisher.Add(publisher_1);
            _context.SaveChanges();


            var publisher = _context.Publisher
                                              .Where(pub => pub.PubId == publisher_1.PubId)
                                              .FirstOrDefault();

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }
        //***************************************************************

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _context.Publisher.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, Publisher publisher)
        {
            if (id != publisher.PubId)
            {
                return BadRequest();
            }

            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
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

        // POST: api/Publishers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Publisher>> PostPublisher(Publisher publisher)
        {
            _context.Publisher.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new { id = publisher.PubId }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Publisher>> DeletePublisher(int id)
        {
            var publisher = await _context.Publisher.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publisher.Remove(publisher);
            await _context.SaveChangesAsync();

            return publisher;
        }

        private bool PublisherExists(int id)
        {
            return _context.Publisher.Any(e => e.PubId == id);
        }
    }
}
