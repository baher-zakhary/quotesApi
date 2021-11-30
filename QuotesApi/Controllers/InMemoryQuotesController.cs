using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InMemoryQuotesController : ControllerBase
    {
        private static readonly List<Quote> _quotesInit = new List<Quote>() {
            new Quote { Id = 0, Author = "Daniel", Description = "Quote Description", Title = "Quote title" }
        };

        static List<Quote> _quotes;

        public InMemoryQuotesController()
        {
            _quotes = _quotesInit;
        }

        [HttpGet]
        public IEnumerable<Quote> Get()
        {
            return _quotes;
        }

        [HttpPost]
        public void Post([FromBody] Quote quote)
        {
            _quotes.Add(quote);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Quote quote)
        {
            _quotes[id] = quote;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _quotes.RemoveAt(id);
        }
    }
}
