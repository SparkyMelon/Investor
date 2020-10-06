using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Investor.Data;
using Investor.Models;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Investor.Controllers
{
    public class LoginController : Controller
    {
        private readonly InvestorContext _context;

        public LoginController(InvestorContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        enum Validation
        {
            VALID = 0,
            INVALID_USERNAME = 1,
            INVALID_EMAIL = 2, 
            INVALID_PASSWORD = 3,
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<int> RegisterValidate(Account newAccount)
        {
            List<Account> accounts = await _context.Account.ToListAsync();

            foreach (Account account in accounts)
            {
                if (account.Email == newAccount.Email)
                {
                    return (int)Validation.INVALID_EMAIL;
                }

                if (account.Username == newAccount.Username)
                {
                    return (int)Validation.INVALID_USERNAME;
                }
            }

            newAccount.Salt = GenerateSalt();
            newAccount.Password = HashPassword(newAccount.Password, newAccount.Salt);

            _context.Add(newAccount);
            await _context.SaveChangesAsync();

            return (int)Validation.VALID;
        }

        [HttpPost]
        public async Task<int> LoginValidate(string usernameEmail, string password)
        {
            List<Account> accounts = await _context.Account.ToListAsync();


            foreach(Account account in accounts)
            {
                if (usernameEmail == account.Email || usernameEmail == account.Username)
                {
                    string hashedPassword = HashPassword(password, account.Salt);

                    if (hashedPassword == account.Password)
                    {
                        return (int)Validation.VALID;
                    }
                    else
                    {
                        return (int)Validation.INVALID_PASSWORD;
                    }
                }
            }

            return (int)Validation.INVALID_USERNAME;
        }

        [HttpPost]
        public IActionResult LoginSuccess(string usernameEmail)
        {
            List<Account> accounts = _context.Account.ToList();
            string actualUsername = "";
            int id = 0;

            foreach (Account account in accounts)
            {
                if (usernameEmail == account.Email || usernameEmail == account.Username)
                {
                    actualUsername = account.Username;
                    id = account.Id;
                }
            }

            HttpContext.Session.SetInt32("Id", id);
            HttpContext.Session.SetString("Username", actualUsername);
            
            return RedirectToAction("Index", "Home");
        }

        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[32];
                rng.GetBytes(randomNumber);
                return randomNumber;
            }
        }

        private string HashPassword(string password, byte[] salt)
        {
            //byte[] salt = Encoding.UTF8.GetBytes(saltStr);

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
