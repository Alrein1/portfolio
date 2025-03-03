using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public int SaveGame(string jsonStateString, string gameConfigName);
    public List<string> GetSavedGames();
    public string DeleteAll();
    public Game LoadGame(string gameName);
}