using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesApi.Data;
using QuotesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class QuotesController : ControllerBase
    {

        private QuotesDbContext _quotesDbContext;

        public QuotesController(QuotesDbContext quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }

        // GET: api/<QuotesController>
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [AllowAnonymous]
        public IActionResult Get(string sort)
        {
            IQueryable<Quote> quotesQuery;
            switch (sort)
            {
                case "desc":
                    quotesQuery = _quotesDbContext.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    quotesQuery = _quotesDbContext.Quotes.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    quotesQuery = _quotesDbContext.Quotes.OrderBy(q => q.CreatedAt);
                    break;
            }
            return Ok(quotesQuery);
            //return StatusCode(StatusCodes.Status200OK, quotesQuery);
        }

        // GET api/<QuotesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quote = _quotesDbContext.Quotes.Find(id);
            if (quote == null)
            {
                //return NotFound();
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(quote);
        }

        // Routing demo
        // GET api/quotes/test/{id}
        [HttpGet("[action]/{id}")]
        public int Test(int id)
        {
            return id;
        }

        [HttpGet("[action]")]
        //[Route("[action]")]
        public IActionResult PagingQuote(int? pageNumber, int? pageSize)
        {
            var quotes = _quotesDbContext.Quotes;

            pageNumber = pageNumber ?? 1;
            pageSize = pageSize ?? 5;

            return Ok(quotes.Skip((int)((pageNumber - 1) * pageSize)).Take((int)pageSize));
        }

        [HttpGet("[action]")]
        public IActionResult SearchQuote(string type)
        {
            var quote = _quotesDbContext.Quotes.Where(q => q.Type.StartsWith(type));
            return Ok(quote);
        }

        [HttpGet("[action]")]
        public IActionResult GetUserQuotes()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var quotes = _quotesDbContext.Quotes.Where(q => q.UserId == userId);
            return Ok(quotes);
        }

        // POST api/<QuotesController>
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            // Make created quote accessible for its creator only
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            quote.UserId = userId;


            //quote.CreatedAt = DateTime.Now;
            _quotesDbContext.Quotes.Add(quote);
            _quotesDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, quote);
        }

        // PUT api/<QuotesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var entity = _quotesDbContext.Quotes.Find(id);
            if (entity == null)
            {
                return NotFound();
            }
            if (userId != entity.UserId)
            {
                return Unauthorized();
            }
            entity.Title = quote.Title;
            entity.Author = quote.Author;
            entity.Description = quote.Description;
            entity.Type = quote.Type;
            entity.CreatedAt = quote.CreatedAt;
            _quotesDbContext.SaveChanges();
            return Ok(entity);
        }

        // DELETE api/<QuotesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            
            var quote = _quotesDbContext.Quotes.Find(id);
            if (quote == null)
            {
                return NotFound();
            }

            if (userId != quote.UserId)
            {
                return Unauthorized();
            }
            _quotesDbContext.Remove(quote);
            _quotesDbContext.SaveChanges();
            return Ok();
        }
    }
}
