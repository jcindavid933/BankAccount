using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bankaccount.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace bankaccount.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
 
        // here we can "inject" our context service into the constructor
        public HomeController(Context context)
        {
            dbContext = context;
        }

        public ViewResult Index()
        {
            return View();
        }
        [HttpGet("login")]
        public ViewResult Login()
        {
            return View();
        }
        [HttpPost("Register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if(dbContext.User.Any(a => a.Email == user.Email))
                {
                    ModelState.AddModelError("email", "Email already exists!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserID", user.UserId);
                    int? logged_in_user = HttpContext.Session.GetInt32("UserID");
                    HttpContext.Session.SetString("Username", user.FirstName);
                    return Redirect($"account/{logged_in_user}");
                }
            }
            return View("Index");
        }
        [HttpPost("Login_user")]
        public IActionResult Login_User(Login_User user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.User.FirstOrDefault(u => u.Email == user.email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("email", "Invalid Email");
                    return View("Login");
                }
                var hasher = new PasswordHasher<Login_User>();
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.password);
                
                if(result == 0)
                {
                    ModelState.AddModelError("password", "Invalid Password");
                    return View("Login");
                }
                else
                {
                    HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                    HttpContext.Session.SetString("Username", userInDb.FirstName);
                    int? logged_in_user = HttpContext.Session.GetInt32("UserID");
                    return Redirect($"account/{logged_in_user}");
                }
            }
            return View("Index");
        }
        [HttpGet("account/{id}")]
        public IActionResult Account(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            int logged_in_user = (int)HttpContext.Session.GetInt32("UserID");
            User current_user = dbContext.User.Include(user => user.Transactions).FirstOrDefault(user => user.UserId == logged_in_user);
            List<Transaction> transactions = current_user.Transactions;
            transactions.Reverse();
            decimal sum = 0;

            foreach (var i in transactions)
            {
                sum += i.Amount;
            }
            ViewBag.sum = sum;
            ViewBag.transactions = transactions;
            ViewBag.username = HttpContext.Session.GetString("Username");
            return View();
        }
        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpPost("transaction")]
        public IActionResult Transaction(Transaction transaction)
        {
            if(ModelState.IsValid)
            {
                int logged_in_user = (int)HttpContext.Session.GetInt32("UserID");
                transaction.UserId = logged_in_user;
                decimal withdraw = transaction.Amount * -1;
                User current_user = dbContext.User.Include(user => user.Transactions).FirstOrDefault(user => user.UserId == logged_in_user);
                List<Transaction> transactions = current_user.Transactions;
                decimal sum = 0;
                foreach (var i in transactions)
                {
                    sum += i.Amount;
                }
                if (withdraw > sum)
                {
                    TempData["Error"] = "You cannot withdraw money that you don't have brotha.";
                    return Redirect($"account/{logged_in_user}");
                }
                dbContext.Add(transaction);
                dbContext.SaveChanges();
                return Redirect($"account/{logged_in_user}");
            }
            else
            {
                return View("Account");
            }
        }

    }
}
