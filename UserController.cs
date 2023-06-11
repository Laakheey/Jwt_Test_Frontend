using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicket.Models;
using Newtonsoft.Json;
using PagedList;
using System.Data;
using System.Net;
using System.Net.Http.Headers;

namespace MovieTicket.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
    
        public async Task<ActionResult> ShowMovies()
        {
            IEnumerable<Movie> movies = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));
                var responseTask = client.GetAsync("api/User/getmovies");
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
                    ModelState.AddModelError(string.Empty, "No Product Found.");
                }
            }

            return View(movies);
        }



        [HttpGet]
        public async Task<IActionResult> Booking(Movie movie)
        {
            var booking = new Booking()
            {
                Id = movie.Id,
                Name = movie.Name,
                Date = movie.Date,
                Genre = movie.Genre,
                Cast = movie.Cast,
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", HttpContext.Session.GetString("Token"));


                using (var response = await client.PostAsJsonAsync("api/User/Booking", booking))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("BookedMovies", "User");
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        if (errorMessage == "Already booked")
                        {
                            TempData["ErrorMessage"] = $"This movie {movie.Id} i already booked.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error occurred while booking the movie.";
                        }

                        return RedirectToAction("ShowMovies", "User");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while booking the movie.";
                        return RedirectToAction("BookedMovies", "User");
                    }
                }
            }

        }






        [HttpGet]
        public async Task<ActionResult> BookedMovies()
        {
            IEnumerable<Booking> bookings = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7257/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));

                var response = await client.GetAsync("api/User/booked");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    bookings = JsonConvert.DeserializeObject<List<Booking>>(content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No Data Found.");
                }
            }

            return View(bookings);
        }






    }
}
