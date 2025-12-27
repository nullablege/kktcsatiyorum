using AutoMapper;
using EntityLayer.Constants;
using EntityLayer.Entities;
using KKTCSatiyorum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<UygulamaKullanicisi> _userManager;
        private readonly SignInManager<UygulamaKullanicisi> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IMapper mapper,
            UserManager<UygulamaKullanicisi> userManager,
            SignInManager<UygulamaKullanicisi> signInManager,
            ILogger<AccountController> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz giriþ denemesi.");
                return View(model);
            }

            // Kullanýcý askýda mý
            if (user.AskidaMi && user.AskidaBitisTarihi.HasValue && user.AskidaBitisTarihi.Value > DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, $"Hesabýnýz {user.AskidaBitisTarihi.Value:dd.MM.yyyy} tarihine kadar askýda.");
                return View(model);
            }
            else if (user.AskidaMi && user.AskidaBitisTarihi.HasValue && user.AskidaBitisTarihi.Value <= DateTime.UtcNow)
            {
                user.AskidaMi = false;
                user.AskidaBitisTarihi = null;
                await _userManager.UpdateAsync(user);
            }

            var username = user.UserName ?? user.Email;
            var result = await _signInManager.PasswordSignInAsync(username!, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Son giriþ tarihini güncelle
                user.SonGirisTarihi = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Kullanýcý giriþ yaptý: {Email}", model.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Kullanýcý hesabý kilitlendi: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Çok fazla baþarýsýz giriþ denemesi. Hesabýnýz geçici olarak kilitlendi.");
                return View(model);
            }
            
            ModelState.AddModelError(string.Empty, "Geçersiz giriþ denemesi.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = _mapper.Map<UygulamaKullanicisi>(model); // Automapper 

            user.UserName ??= model.Email;
            user.EmailConfirmed = true; //Email doðrulamasý 

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleNames.User );

                _logger.LogInformation("Yeni kullanýcý oluþturuldu: {Email}", user.Email);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Kullanýcý çýkýþ yaptý.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
