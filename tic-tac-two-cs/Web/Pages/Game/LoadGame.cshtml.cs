using System.Globalization;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Services;

namespace Web.Pages.Game;

public class LoadGameModel : PageModel
{
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;
    private readonly IGameService _gameService;
    private readonly ILogger<LoadGameModel> _logger;

    public List<string> SavedGames { get; set; } = new();
    
    public LoadGameModel(
        IGameRepository gameRepository,
        IConfigRepository configRepository,
        IGameService gameService,
        ILogger<LoadGameModel> logger)
    {
        _gameRepository = gameRepository;
        _gameService = gameService;
        _logger = logger;
    }
    
    public void OnGet()
    {
        SavedGames = _gameRepository.GetSavedGames();
    }

    public IActionResult OnPost(string gameName, string playerPiece)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var success = _gameService.LoadGame(gameName, playerPiece == "X");
            if (success)
            {
                TempData["GamePassword"] = "lelelelele";
                TempData["FromLocalSave"] = true;
                return RedirectToPage("./Play", new { gameName });
            }

            ModelState.AddModelError("", "Failed to load game");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading game");
            ModelState.AddModelError("", "Error loading game");
            return Page();
        }
    }

    public string GetSaveTime(string gameName)
    {
        // Extract datetime from game name format: configname_yyyy-MM-dd_HH-mm-ss
        var parts = gameName.Split('_');
        if (parts.Length >= 3)
        {
            var dateStr = $"{parts[^3]}_{parts[^2]}_{parts[^1]}";
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd_HH-mm-ss", 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date.ToString("g");
            }
        }
        return "Unknown";
    }
}