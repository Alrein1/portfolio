using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameBrain;
using Web.Services;

namespace Web.Pages.Game;

public class NewModel : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameService _gameService;
    [BindProperty] 
    public GameMode GameMode { get; set; } = default!;
    [BindProperty]
    public string Password { get; set; } = default!;
    [BindProperty]
    public string SelectedConfigId { get; set; } = default!;

    public List<GameConfiguration> Configurations { get; set; } = default!;

    private readonly ILogger<NewModel> _logger;

    public NewModel(
        IConfigRepository configRepository, 
        IGameService gameService,
        ILogger<NewModel> logger)
    {
        _configRepository = configRepository;
        _gameService = gameService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        try
        {
            _logger.LogInformation("Fetching configurations...");
            Configurations = _configRepository.GetConfigurationNames()
                .Select(name => _configRepository.GetConfigurationByName(name))
                .ToList();
            _logger.LogInformation($"Found {Configurations.Count} configurations");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configurations");
            throw;
        }
    }

    public IActionResult OnPost()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var config = _configRepository.GetConfigurationByName(SelectedConfigId);
            if (config == null)
            {
                _logger.LogError($"Configuration not found: {SelectedConfigId}");
                ModelState.AddModelError("", "Selected configuration not found.");
                return Page();
            }
            try
            {
                var gameName = _gameService.CreateNewGame(config, Password, GameMode);
                TempData["GamePassword"] = Password;
                TempData["GameMode"] = GameMode.ToString();
                return RedirectToPage("./Play", new { gameName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
    }
    
}