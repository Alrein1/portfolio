using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameBrain;
using Web.Services;

namespace Web.Pages.Game;

public class PlayModel : PageModel
{
    private readonly IGameService _gameService;
    private readonly ILogger<PlayModel> _logger;

    [BindProperty(SupportsGet = true)]
    public string GameName { get; set; } = default!;
    
    public string Password { get; set; } = default!;
    public TicTacTwoBrain? Game { get; private set; }

    public PlayModel(IGameService gameService, ILogger<PlayModel> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }


    public IActionResult OnGet()
    {
        _logger.LogInformation($"Accessing Play page with GameName: {GameName}");
        
        Password = TempData["GamePassword"] as string ?? "";
        bool fromLocalSave = TempData["FromLocalSave"] as bool? == true;
        _logger.LogInformation($"Password from TempData exists: {!string.IsNullOrEmpty(Password)}");

        if (!fromLocalSave && string.IsNullOrEmpty(Password))
        {
            _logger.LogWarning("No password found, redirecting to Join");
            return RedirectToPage("/Game/Join");
        }

        Game = _gameService.GetGame(GameName);
        _logger.LogInformation($"Game found: {Game != null}");

        if (Game == null)
        {
            _logger.LogWarning($"Game not found: {GameName}");
            TempData["Error"] = "Game not found";
            return RedirectToPage("/Game/Index");
        }

        return Page();
    }

    public IActionResult OnPostSave()
    {
        _gameService.SaveGame(GameName);
        return RedirectToPage("./Index");
    }
}