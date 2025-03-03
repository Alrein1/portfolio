using GameBrain;
using Microsoft.AspNetCore.SignalR;
using Web.Models;
using Web.Services;

namespace Web.Hubs;

public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameHub> _logger;
    
    public GameHub(IGameService gameService, ILogger<GameHub> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public async Task JoinGame(string gameName, string password)
    {
        try
        {
            _logger.LogInformation($"Attempting to join game: {gameName}");
            var gameMode = _gameService.GetGameMode(gameName);
            _logger.LogInformation($"Game mode: {gameMode}");

            // Join the game's SignalR group regardless of game mode
            await Groups.AddToGroupAsync(Context.ConnectionId, gameName);

            // For AI vs AI, join as spectator
            if (gameMode == GameMode.AIVsAI)
            {
                await Clients.Caller.SendAsync("PlayerJoined", Context.ConnectionId, "Spectator");
                await Clients.Group(gameName).SendAsync("GameReady");
                await MakeAIMove(gameName);
                return;
            }

            // For other modes, try to join as a player
            var (success, piece) = _gameService.JoinGame(gameName, password, Context.ConnectionId);
            if (!success)
            {
                throw new HubException("Unable to join game - invalid password or game is full");
            }

            await Clients.Caller.SendAsync("PlayerJoined", Context.ConnectionId, piece.ToString());

            // Game is ready if it's PvAI or if it's PvP and full
            if (gameMode == GameMode.PlayerVsAI || _gameService.IsGameFull(gameName))
            {
                _logger.LogInformation("Game is ready to start");
                await Clients.Group(gameName).SendAsync("GameReady");
                
                // If it's AI's turn in PvAI mode, make AI move
                if (_gameService.IsAITurn(gameName))
                {
                    await MakeAIMove(gameName);
                }
            }
            else
            {
                _logger.LogInformation("Waiting for other player to join");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error joining game: {gameName}");
            throw;
        }
    }
    public async Task SaveGame(string gameName)
    {
        try
        {
            _logger.LogInformation($"Attempting to save game: {gameName}");
        
            var game = _gameService.GetGame(gameName); 
            if (game == null)
            {
                _logger.LogWarning($"Game not found: {gameName}");
                throw new HubException("Game not found");
            }

            try
            {
                _gameService.SaveGame(gameName);
                await Clients.Caller.SendAsync("GameSaved", "Game saved successfully");
                _logger.LogInformation($"Game {gameName} saved successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error saving game");
                throw new HubException(ex.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving game");
            throw new HubException($"Error saving game: {ex.Message}");
        }
    }
    public async Task MakeMove(string gameName, int x, int y)
    {
        try
        {
            _logger.LogInformation($"Player {Context.ConnectionId} attempting move at ({x},{y}) in game {gameName}");
            var result = _gameService.MakeMove(gameName, x, y, Context.ConnectionId);
            
            if (result.Success)
            {
                await Clients.Group(gameName).SendAsync("MoveMade", result);
                
                // Check if it's AI's turn after the player's move
                if (result.Success && _gameService.IsAITurn(gameName))
                {
                    _logger.LogInformation("Player move successful, triggering AI move");
                    await MakeAIMove(gameName);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("MoveError", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making move");
            await Clients.Caller.SendAsync("MoveError", "An error occurred while making the move");
        }
    }

    public async Task MovePiece(string gameName, int fromX, int fromY, int toX, int toY)
    {
        try
        {
            _logger.LogInformation($"Player {Context.ConnectionId} attempting to move piece from ({fromX},{fromY}) to ({toX},{toY}) in game {gameName}");
            
            var result = _gameService.MovePiece(gameName, fromX, fromY, toX, toY, Context.ConnectionId);
            if (result.Success)
            {
                await Clients.Group(gameName).SendAsync("PieceMoved", result);
                if (result.Success && _gameService.IsAITurn(gameName))
                {
                    _logger.LogInformation("Player move successful, triggering AI move");
                    await MakeAIMove(gameName);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("MoveError", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error moving piece in game {gameName}");
            await Clients.Caller.SendAsync("MoveError", "An error occurred while moving the piece");
        }
    }

    public async Task MoveGrid(string gameName, int newX, int newY)
    {
        try
        {
            _logger.LogInformation($"Player {Context.ConnectionId} attempting to move grid to ({newX},{newY}) in game {gameName}");
            
            var result = _gameService.MoveGrid(gameName, newX, newY, Context.ConnectionId);
            if (result.Success)
            {
                await Clients.Group(gameName).SendAsync("GridMoved", result);
                if (result.Success && _gameService.IsAITurn(gameName))
                {
                    _logger.LogInformation("Player move successful, triggering AI move");
                    await MakeAIMove(gameName);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("MoveError", result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error moving grid in game {gameName}");
            await Clients.Caller.SendAsync("MoveError", "An error occurred while moving the grid");
        }
    }
    public async Task MakeAIMove(string gameName)
    {
        try
        {
            _logger.LogInformation($"AI attempting move in game: {gameName}");
            var game = _gameService.GetGame(gameName);
            
            if (game == null)
            {
                _logger.LogWarning("Game not found");
                return;
            }

            if (!_gameService.IsAITurn(gameName))
            {
                _logger.LogWarning("Not AI's turn");
                return;
            }

            // Add small delay for better UX
            await Task.Delay(1000);

            game.MakeAIMove();
            _logger.LogInformation("AI move made");

            var result = new GameMoveResult
            {
                Success = true,
                Board = game.GameBoard,
                NextPlayer = game.GetNextMoveBy().ToString(),
                GameStatus = game.CheckGameStatus(),
                GridPosition = new GridPosition(){ X= game.GridPosition.x,Y= game.GridPosition.y },
                IsAITurn = _gameService.IsAITurn(gameName)
            };

            await Clients.Group(gameName).SendAsync("MoveMade", result);

            // If it's still AI's turn (in case of AI vs AI), make another move
            if (result.IsAITurn && result.GameStatus == GameStatus.Ongoing)
            {
                await MakeAIMove(gameName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making AI move");
        }
    }
}