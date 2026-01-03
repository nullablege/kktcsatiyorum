using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Controllers
{
    [Authorize(Policy = "ModeratorOnly")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        // Ana panel
        public IActionResult Index()
        {
            return View();
        }

        // Ýlan moderasyon sayfasý
        public IActionResult IlanModerasyon()
        {
            // Bekleyen ilanlarý listele
            return View();
        }

        // Ýlan onaylama
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IlanOnayla(int ilanId)
        {
            // Ýlaný onayla
           // Domaýn ýcerýsýne alacagýz
            
            _logger.LogInformation("Ýlan onaylandý: {IlanId}", ilanId);
            return RedirectToAction(nameof(IlanModerasyon));
        }

        // Ýlan reddetme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IlanReddet(int ilanId, string redNedeni)
        {
            // Ýlaný reddet
            // Domaýn ýcerýsýne alacagýz

            _logger.LogInformation("Ýlan reddedildi: {IlanId}, Neden: {RedNedeni}", ilanId, redNedeni);
            return RedirectToAction(nameof(IlanModerasyon));
        }

        // Þikayet moderasyonu
        public IActionResult SikayetModerasyon()
        {
            // Bekleyen þikayetleri listele
            return View();
        }

        // Kullanýcý yönetimi (sadece Admin)
        [Authorize(Policy = "AdminOnly")]
        public IActionResult KullaniciYonetimi()
        {
            return View();
        }

        // Kullanýcý askýya alma (sadece Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult KullaniciAskiyaAl(string kullaniciId, DateTime bitisTarihi)
        {
            // Kullanýcýyý askýya al
            // Domaýn ýcerýsýne alacagýz

            _logger.LogInformation("Kullanýcý askýya alýndý: {KullaniciId}, Bitiþ: {BitisTarihi}", kullaniciId, bitisTarihi);
            return RedirectToAction(nameof(KullaniciYonetimi));
        }
    }
}
