using System.Collections.Generic;
using System.Linq;

// ============================================================
// GAME STATE
// ============================================================
public enum GamePhase
{
    WaitingToStart,
    TurnInProgress,
    RoundOver,
    GameOver
}

public class GameState
{
    public List<Player> Players = new List<Player>();
    public int CurrentTurnIndex = 0;
    public int RoundNumber = 1;
    public GamePhase Phase = GamePhase.WaitingToStart;
    public Weapon Weapon;

    public GameState(List<Player> players, int liveCount = 2, int blankCount = 4)
    {
        Players = players;
        Weapon = new Weapon(liveCount, blankCount);
    }

    public Player CurrentPlayer => Players[CurrentTurnIndex];

    public List<Player> AlivePlayers => Players.Where(p => p.IsAlive).ToList();
}
