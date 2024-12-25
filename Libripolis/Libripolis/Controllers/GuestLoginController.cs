using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Libripolis.Controllers
{
    public class GuestLoginController : Controller
    {
        private readonly IMemoryCache _cache; // Tymczasowe przechowywanie kodu
        private readonly SignInManager<IdentityUser> _signInManager;

        public GuestLoginController(IMemoryCache cache, SignInManager<IdentityUser> signInManager)
        {
            _cache = cache;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Wyświetla stronę z opcją logowania jako Gość
        }

        [HttpPost]
        public IActionResult GenerateOtp()
        {
            var otp = new Random().Next(100000, 999999).ToString(); // 6-cyfrowy OTP
            var otpExpiry = DateTime.Now.AddMinutes(5); // Kod ważny 5 minut

            _cache.Set("GuestOtp", otp, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = otpExpiry
            });

            ViewBag.Otp = otp; // Wyświetlenie kodu na stronie
            ViewBag.OtpExpiry = otpExpiry;

            return View("EnterOtp");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string inputOtp)
        {
            if (_cache.TryGetValue("GuestOtp", out string storedOtp))
            {
                if (storedOtp == inputOtp)
                {
                    // Znajdź lub utwórz konto gościa
                    var guestUser = await _signInManager.UserManager.FindByNameAsync("Guest");
                    if (guestUser == null)
                    {
                        guestUser = new IdentityUser { UserName = "Guest" };
                        await _signInManager.UserManager.CreateAsync(guestUser);
                    }

                    await _signInManager.SignInAsync(guestUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Nieprawidłowy kod lub kod wygasł.");
            return View("EnterOtp");
        }
    }
}
