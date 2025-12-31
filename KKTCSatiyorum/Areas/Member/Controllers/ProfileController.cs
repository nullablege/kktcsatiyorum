
using BusinessLayer.Features.Member.DTOs;
using BusinessLayer.Features.Member.Services;
using EntityLayer.Constants;
using KKTCSatiyorum.Areas.Member.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KKTCSatiyorum.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize(Roles = RoleNames.User)]
    public class ProfileController : Controller
    {
        private readonly IMemberService _memberService;

        public ProfileController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var result = await _memberService.GetMyProfileAsync(userId);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            var model = new ProfileEditViewModel
            {
                AdSoyad = result.Data.AdSoyad,
                Email = result.Data.Email,
                PhoneNumber = result.Data.PhoneNumber,
                ProfilFotografiYolu = result.Data.ProfilFotografiYolu
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account", new { area = "" });

            var request = new UpdateProfileRequest
            {
                AdSoyad = model.AdSoyad,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _memberService.UpdateMyProfileAsync(userId, request);
            if (result.IsSuccess)
            {
                model.StatusMessage = "Profiliniz başarıyla güncellendi.";
                 var profileResult = await _memberService.GetMyProfileAsync(userId);
                 if(profileResult.IsSuccess)
                 {
                     model.AdSoyad = profileResult.Data.AdSoyad;
                     model.Email = profileResult.Data.Email;
                     model.PhoneNumber = profileResult.Data.PhoneNumber;
                     model.ProfilFotografiYolu = profileResult.Data.ProfilFotografiYolu;
                 }
                return View(model);
            }

            ModelState.AddModelError(string.Empty, result.Error.Message);
            return View(model);
        }
    }
}
