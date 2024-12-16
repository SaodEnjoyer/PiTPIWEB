using Microsoft.AspNetCore.Mvc;
using PiTPIWEB.Interfaces;
using Model;

namespace PiTPIWEB.Controllers
{
    public class MVCAuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public MVCAuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: MVCAuthors
        public async Task<IActionResult> Index()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return View(authors);
        }

        // GET: MVCAuthors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // GET: MVCAuthors/Create
        public IActionResult Create()
        {
            return View();
        }

    }
}
