using BooksAPI.DTOs;
using BooksAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http;
using System.Web.Http.Description;



namespace BooksAPI.Controllers
{
    public class BooksController : ApiController
    {
        private BooksAPIContext db = new BooksAPIContext();

        // lambda expression for select() method
        private static readonly Expression<Func<Book, BookDto>> AsBookDto =
            x => new BookDto
            {
                Title = x.Title,
                Author = x.Author.Name,
                Genre = x.Genre
            };



        //empty request
        [Route("")]
        [HttpGet]
        public HttpResponseMessage message()
        {


            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("Welcome to Books API");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }

        // GET: api/Books
        public IQueryable<BookDto> GetBooks()
        {
            // return db.Books;
            return db.Books.Include(b => b.Author).Select(AsBookDto);


        }



        // GET: api/Books/5
        [ResponseType(typeof(BookDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            BookDto book = await db.Books.Include(b => b.Author)
                .Where(b => b.BookId == id)
                .Select(AsBookDto)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

       


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

      
    }
}