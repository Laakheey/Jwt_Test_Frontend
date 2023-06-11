using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicket.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using X.PagedList;

namespace MovieTicket.Controllers
{

    public class AdminController : Controller
    {

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            IEnumerable<Movie> movies = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var responseTask = client.GetAsync("api/Admin/getmovies");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Movie>>(readTask.Result);
                    readTask.Wait();
                    movies = deserialized;
                }
                else
                {
                    movies = Enumerable.Empty<Movie>();
                    ModelState.AddModelError(string.Empty, "No Movies Found.");
                }
            }
            ViewBag.username = TempData["username"];
            return View(movies);
        }






        [HttpGet]
        public async Task<ActionResult> AddMovie()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> AddMovie(AddMovieRequest addMovieRequest)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));

                var postTask = await client.PostAsJsonAsync<AddMovieRequest>("api/Admin/addmovie", addMovieRequest);
                if (postTask.IsSuccessStatusCode)
                {
                    var response = await postTask.Content.ReadAsStringAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorResponse = await postTask.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Not Added: {errorResponse}");
                }

                return View();
            }
        }







    }

}
