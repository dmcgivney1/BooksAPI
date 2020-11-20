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
using System.Web.Http.Description;



namespace BooksAPI.Controllers
{
    [RoutePrefix("api/books")]
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



        ////empty request
        //[Route("~/")]
        //[HttpGet]
        //public HttpResponseMessage message()
        //{


        //    var response = new HttpResponseMessage(HttpStatusCode.OK);
        //    response.Content = new StringContent("Welcome to Books API");
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        //    return response;
        //}

        // GET: api/Books
        [Route("")]
        public IQueryable<BookDto> GetBooks()
        {
            // return db.Books;
            return db.Books.Include(b => b.Author).Select(AsBookDto);


        }



        // GET: api/Books/5
        [Route("{id:int}")]
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

        [Route("{id:int}/details")]
        [ResponseType(typeof(BookDetailDto))]
        public async Task<IHttpActionResult> getBookDetail(int id)
        {
            var book = await (from b in db.Books.Include(b => b.Author)
                              where b.BookId == id
                              select new BookDetailDto
                              {
                                  Title = b.Title,
                                  Genre = b.Genre,
                                  PublishDate = b.PublishDate,
                                  Price = b.Price,
                                  Description = b.Description,
                                  Author = b.Author.Name
                              }).FirstOrDefaultAsync();

            if (book == null)
            { return NotFound(); }

            return Ok(book);

        }

        [Route("{genre:alpha}")]
        public IQueryable<BookDto> GetBooksByGenre(string genre)
        {
            return db.Books.Include(b => b.Author)
                .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .Select(AsBookDto);

        }


        [Route("~/api/authors/{authorId:int}/Books")]
        public IQueryable<BookDto> GetBooksByAuthor(int authorId)
        {
            return db.Books.Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .Select(AsBookDto);
        }

        [Route("date/{pubdate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
        [Route("date/{*pubdate:datetime:regex(\\d{4}/\\d{2}/\\d{2})}")]  // new
        public IQueryable<BookDto> GetBooks(DateTime pubdate)
        {
            return db.Books.Include(b => b.Author)
                  .Where(b => DbFunctions.TruncateTime(b.PublishDate)
                      == DbFunctions.TruncateTime(pubdate))
                  .Select(AsBookDto);

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