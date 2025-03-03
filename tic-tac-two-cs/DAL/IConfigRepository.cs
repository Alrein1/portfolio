using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    List<string> GetConfigurationNames();
    GameConfiguration GetConfigurationByName(string name);
    GameConfiguration GetConfigurationById(int id);
    void DeleteConfiguration(string configName);
    void UpdateConfiguration(GameConfiguration config);


    void SaveConfiguration(GameConfiguration gameConfig);
}
