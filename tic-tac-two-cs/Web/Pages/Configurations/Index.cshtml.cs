using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameBrain;
using DAL;

namespace Web.Pages.Configurations;

public class IndexModel : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly ILogger<IndexModel> _logger;

    public List<GameConfiguration> Configurations { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public IndexModel(IConfigRepository configRepository, ILogger<IndexModel> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    public void OnGet()
    {
        Configurations = _configRepository.GetConfigurationNames()
            .Select(name => _configRepository.GetConfigurationByName(name))
            .ToList();
    }

    public IActionResult OnPostDelete(string configName)
    {
        try
        {
            _configRepository.DeleteConfiguration(configName);
            TempData["Message"] = $"Configuration '{configName}' deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            _logger.LogWarning(ex, "Attempted to delete configuration in use");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while deleting the configuration.";
            _logger.LogError(ex, "Error deleting configuration");
        }
        return RedirectToPage();
    }
}