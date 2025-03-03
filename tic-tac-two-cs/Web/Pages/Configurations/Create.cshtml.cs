using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameBrain;
using DAL;

namespace Web.Pages.Configurations;

public class CreateModel : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public ConfigurationDto Config { get; set; } = new()
    {
        BoardSizeWidth = 5,
        BoardSizeHeight = 5,
        WinCondition = 3,
        MovePieceAfterNMoves = 4
    };

    public CreateModel(IConfigRepository configRepository, ILogger<CreateModel> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    public IActionResult OnPost()
    {
        // Additional validation for configuration name
        if (string.IsNullOrWhiteSpace(Config.Name))
        {
            ModelState.AddModelError("Config.Name", "Name is required");
            return Page();
        }

        // Check if configuration with this name already exists
        var existingNames = _configRepository.GetConfigurationNames();
        if (existingNames.Contains(Config.Name))
        {
            ModelState.AddModelError("Config.Name", "A configuration with this name already exists");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Validate win condition
            var maxWin = Math.Min(Config.BoardSizeWidth, Config.BoardSizeHeight);
            if (Config.WinCondition > maxWin)
            {
                ModelState.AddModelError("Config.WinCondition", 
                    "Win condition cannot be larger than the smallest board dimension");
                return Page();
            }

            // Create the configuration
            var gameConfig = Config.ToGameConfiguration();
            _configRepository.SaveConfiguration(gameConfig);

            TempData["Message"] = $"Configuration '{Config.Name}' created successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating configuration");
            ModelState.AddModelError("", $"Error creating configuration: {ex.Message}");
            return Page();
        }
    }
}