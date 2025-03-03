using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Game;
public class JoinModel : PageModel
{
    private readonly IGameService _gameService;
    private readonly ILogger<JoinModel> _logger;

    [BindProperty]
    public string Password { get; set; } = default!;

    public JoinModel(IGameService gameService, ILogger<JoinModel> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var gameName = _gameService.FindGameByPassword(Password);
        if (gameName == null)
        {
            ModelState.AddModelError("", "No game found with this password");
            return Page();
        }

        // Don't join the game here, just pass the information to the Play page
        TempData["GamePassword"] = Password;
        return RedirectToPage("./Play", new { gameName });
    }
}