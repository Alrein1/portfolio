using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameBrain;
using DAL;

namespace Web.Pages.Configurations;

public class EditModel : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public ConfigurationDto Config { get; set; } = default!;

    public EditModel(IConfigRepository configRepository, ILogger<EditModel> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    public IActionResult OnGet(string name)
    {
        var config = _configRepository.GetConfigurationByName(name);
        if (config == null)
        {
            return NotFound();
        }

        Config = ConfigurationDto.FromGameConfiguration(config);
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Validate win condition
            var maxWin = Math.Min((int) Config.BoardSizeWidth, (int) Config.BoardSizeHeight);
            if (Config.WinCondition > maxWin)
            {
                ModelState.AddModelError("Config.WinCondition", 
                    "Win condition cannot be larger than the smallest board dimension");
                return Page();
            }

            _configRepository.UpdateConfiguration(Config.ToGameConfiguration());
            TempData["Message"] = "Configuration updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration");
            ModelState.AddModelError("", $"Error updating configuration: {ex.Message}");
            return Page();
        }
    }
}