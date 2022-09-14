using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace MasCom.Serverv2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadClientAsync()
        {
            var fs = new FileStream("wwwroot/Client.zip", FileMode.Open, FileAccess.Read);
            await Task.CompletedTask;
            return File(fs, "application/zip", "MasComClient.zip");
        }
    }
}
