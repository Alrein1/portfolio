using System.Net.NetworkInformation;
using GameBrain;

namespace DAL;

public class ConfigRepositoryHardcoded: IConfigRepository
{
    private List<GameConfiguration> _gameConfigurations = new List<GameConfiguration>()
    {
        new GameConfiguration()
        {
            Name = "Classical"
        },
        new GameConfiguration()
        {
            Name = "Big board",
            BoardSizeWidth = 10,
            BoardSizeHeight = 10,
            WinCondition = 4,
            MovePieceAfterNMoves = 3,
        },
    };

    public GameConfiguration GetConfigurationById(int id)
    {
        throw new NetworkInformationException();
    }
    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations
            .OrderBy(x => x.Name)
            .Select(config => config.Name)
            .ToList();
    }


    public GameConfiguration GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.Name == name);
    }

    public void SaveConfiguration(GameConfiguration gameConfig)
    {
    }

    public void DeleteConfiguration(string configName)
    {
        throw new NotImplementedException();
    }

    public void UpdateConfiguration(GameConfiguration config)
    {
            throw new NotImplementedException();
    }
}