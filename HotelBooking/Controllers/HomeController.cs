using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelBooking.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HotelBooking.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private WdaContext _context;
        private  UserManager<User> _userManager;
        private  SignInManager<User> _signInManager;

        public HomeController(WdaContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var roomsearch = new RoomSearch
            {
                Cities = (_context.Room.Select(t => t.City).Distinct()).ToList(),
                RoomTypes = (_context.RoomType.Select(t => t.RoomType1)).ToList()
            };
            return View(roomsearch);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Results(IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                DateTime checkin;
                DateTime checkout;
                bool dayin = DateTime.TryParseExact(form["datein"].ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out checkin);
                bool dayout = DateTime.TryParseExact(form["dateout"].ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out checkout);
                if (dayin && dayout && checkout>checkin)
                {
                    int minprice = 0;
                    int maxprice = 500;
                    var selectedCity = form["city"].ToString();
                    var selectedRoomtype = form["roomtype"].ToString();
                    try
                    {
                        var splitedrangeprice = form["rangeprice"].ToString().Split(",");
                        minprice = Convert.ToInt16(splitedrangeprice[0]);
                        maxprice = Convert.ToInt16(splitedrangeprice[1]);
                    }
                    catch (Exception)
                    {
                        //nothing happens -- get default values 0 - 500
                    }
                    var r1 = from r in _context.Room
                             join s in _context.RoomType on r.RoomType equals s.Id
                             join b in _context.Bookings on r.RoomId equals b.RoomId
                             where r.City == selectedCity && s.RoomType1 == selectedRoomtype && r.Price <= maxprice && r.Price >= minprice
                             select new {r.RoomId, b.CheckInDate,b.CheckOutDate};
                    List<int> BookedRooms = new List<int>();
                    foreach (var book in r1)
                    {
                        try
                        {
                            DateTime bookin = DateTime.ParseExact(book.CheckInDate, "dd/MM/yyyy", null);
                            DateTime bookout = DateTime.ParseExact(book.CheckOutDate, "dd/MM/yyyy", null);
                            if (!(checkout<=bookin || checkin >= bookout))
                            {
                                if (!BookedRooms.Contains(book.RoomId))
                                {
                                    BookedRooms.Add(book.RoomId);
                                }
                                
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    var roomsupersearch = new RoomSuperSearch
                    {
                        RoomModels = from r in _context.Room
                                join s in _context.RoomType on r.RoomType equals s.Id
                                where r.City == selectedCity && s.RoomType1 == selectedRoomtype && r.Price <= maxprice && r.Price >= minprice
                                select new RoomModel
                                {
                                    Room = r,
                                    Booked = BookedRooms.Contains(r.RoomId)
                                },

                        Roomsearch =  new RoomSearch
                        {
                            Cities = (_context.Room.Select(t => t.City).Distinct()).ToList(),
                            RoomTypes = (_context.RoomType.Select(t => t.RoomType1)).ToList()
                        },
                        CheckInDate = checkin.ToShortDateString(),
                        CheckOutDate = checkout.ToShortDateString(),
                        SelectedCity = selectedCity,
                        SelectedRoomType = selectedRoomtype,
                        MinPrice=minprice,
                        MaxPrice=maxprice
                    };
                    return View(roomsupersearch);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public IActionResult Book(int roomId, DateTime datein, DateTime dateout)
        {
            if (ModelState.IsValid)
                {
                    var bookingModel = new BookingModel
                {
                    Room = _context.Room.Where(r => r.RoomId == roomId).First(),
                    ReviewModel = from r in _context.Reviews
                                  join u in _context.User on r.UserId equals u.UserId
                                  where r.RoomId == roomId
                                  select new ReviewModel
                                  {
                                      Username = u.Username,
                                      Rate = r.Rate,
                                      Text = r.Text,
                                      DateCreated = r.DateCreated
                                  },
                    CheckInDate = datein,
                    CheckOutDate = dateout,
                    RoomDescr = (from r in _context.RoomType
                                join s in _context.Room on r.Id equals s.RoomType
                                where s.RoomId == roomId
                                select r.RoomType1).First().ToString()
                };
                return View(bookingModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult BookRegister(int roomId, DateTime datein, DateTime dateout)
        {
            int userId = Convert.ToInt16(_userManager.GetUserId(HttpContext.User));

            if (ModelState.IsValid && datein<dateout && datein>=DateTime.Today.ToLocalTime())
            {
                var entry = new Bookings
                {
                    RoomId = roomId,
                    UserId = userId,
                    CheckInDate = datein.ToString("dd/MM/yyyy"),
                    CheckOutDate = dateout.ToString("dd/MM/yyyy")
                };
                if (_context.Bookings.Where(t => t.RoomId == roomId && t.UserId == userId && t.CheckInDate == datein.ToString("dd/MM/yyyy") && t.CheckOutDate == dateout.ToString("dd/MM/yyyy")).Count()==0)
                {
                    _context.Bookings.Add(entry);
                    _context.SaveChanges();
                }

                return RedirectToAction("Profile", "Home");
            }
            else
            {
                return RedirectToAction("Book", "Home", new
                {
                    roomId = roomId,
                    datein = datein,
                    dateout = dateout
                });
            }
        }
        public IActionResult Profile()
        {
            try
            {
                int userId = Convert.ToInt16(_userManager.GetUserId(HttpContext.User));

                var userprof = new UserProfile
                {
                    sbookings = from b in _context.Bookings
                                join r in _context.Room on b.RoomId equals r.RoomId
                                join t in _context.RoomType on r.RoomType equals t.Id
                                where b.UserId == userId
                                select new SuperBooking {
                                    BookingId = b.BookingId,
                                    CheckInDate = b.CheckInDate,
                                    CheckOutDate = b.CheckOutDate,
                                    RoomId = b.RoomId,
                                    Name = r.Name,
                                    City = r.City,
                                    Area = r.Area,
                                    Photo = r.Photo,
                                    RoomType = t.RoomType1,
                                    Price = r.Price,
                                    ShortDescription = r.ShortDescription },
                    sreviews = from a in _context.Reviews
                               join r in _context.Room on a.RoomId equals r.RoomId
                               where a.UserId == userId
                               select new SuperReview
                               {
                                   Review = a,
                                   Room = r
                               },
                    sfavorites = from f in _context.Favorites
                                 join r in _context.Room on f.RoomId equals r.RoomId
                               where f.UserId == userId && f.Status==1
                               select new SuperFavorite
                               {
                                   Favorite = f,
                                   Room = r
                               }
                };
                return View(userprof);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<int> GetFavorite(int roomId)
        {
            int userId = Convert.ToInt16(_userManager.GetUserId(HttpContext.User));
            int Cnt = await _context.Favorites.Where(t => t.RoomId == roomId && t.UserId == userId && t.Status == 1).CountAsync();
            return Cnt;
        }
        public async Task SetFavorite(int roomId, int status)
        {
            int userId =Convert.ToInt16(_userManager.GetUserId(HttpContext.User));
            var FavCnt = await _context.Favorites.Where(t => t.RoomId == roomId && t.UserId == userId).CountAsync();

            if (FavCnt == 0 )
            {
                var entry = new Favorites
                {
                    Status = status,
                    RoomId = roomId,
                    UserId = userId
                };
                await _context.Favorites.AddAsync(entry);
            }
            else
            {
                var Fav = await _context.Favorites.SingleAsync(t => t.RoomId == roomId && t.UserId == userId);
                Fav.Status = status;
                 _context.Favorites.Update(Fav);

            }
            await _context.SaveChangesAsync();

        }
        
        public async Task InsertReview(string rate, string text, string roomid)
        {
            int userId = Convert.ToInt16(_userManager.GetUserId(HttpContext.User));
            var entry = new Reviews
                {
                    Rate = Convert.ToInt16(rate),
                    Text = text,
                    RoomId = Convert.ToInt16(roomid),
                    UserId = userId
            };
            await _context.Reviews.AddAsync(entry);

            await _context.SaveChangesAsync();

        }
    }
}
