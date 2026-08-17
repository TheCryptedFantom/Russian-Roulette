using System.Collections.Generic;
using UnityEngine;

// ============================================================
// SHOT RESULT (so the UI can read exactly what happened, without
// re-deriving it from game state)
// ============================================================
public struct ShotResult
{
    public Player Shooter;
    public Player Target;
    public bool WasLive;
    public bool TargetEliminated;
    public bool WentAgain; // true if shooter shot themselves with a blank

    public int LiveRemaining;
    public int BlankRemaining;
}

// ============================================================
// TURN MANAGER
// ============================================================
public class TurnManager : MonoBehaviour
{
    public GameState State;

    // Call this to set up a new game. liveCount/blankCount control the shell mix
    // (default 2 live / 4 blank = a standard 6-shell load).
    public void StartNewGame(List<Player> players, int liveCount = 2, int blankCount = 4)
    {
        State = new GameState(players, liveCount, blankCount);
        State.Phase = GamePhase.TurnInProgress;
        State.CurrentTurnIndex = 0;
        Debug.Log($"Game started with {players.Count} players. " +
                   $"{liveCount} live / {blankCount} blank shells loaded.");
    }

    // Core mechanic: current player picks a target (self or another player) and shoots.
    // Returns a ShotResult describing exactly what happened, for the UI to display.
    public ShotResult TakeShot(Player target)
    {
        var result = new ShotResult();
        if (State.Phase != GamePhase.TurnInProgress) return result;
        if (target == null || !target.IsAlive) return result;

        Player shooter = State.CurrentPlayer;
        bool isSelfShot = target == shooter;
        bool isLive = State.Weapon.PullTrigger();

        result.Shooter = shooter;
        result.Target = target;
        result.WasLive = isLive;

        if (isLive)
        {
            target.Health -= 1;
            Debug.Log($"{shooter.Name} shot {target.Name} — LIVE round. {target.Name} has {target.Health} health left.");

            if (target.Health <= 0)
            {
                target.IsAlive = false;
                result.TargetEliminated = true;
                Debug.Log($"{target.Name} was eliminated.");
            }
        }
        else
        {
            Debug.Log($"{shooter.Name} shot at {target.Name} — blank. No damage.");
        }

        CheckWinCondition();

        if (State.Phase == GamePhase.TurnInProgress)
        {
            // Live round: always advance to the next player.
            // Blank round: advance UNLESS the shooter shot themselves, in which case they go again.
            if (isLive || !isSelfShot)
            {
                AdvanceTurn();
            }
            else
            {
                result.WentAgain = true;
            }
        }

        result.LiveRemaining = State.Weapon.LiveRemaining;
        result.BlankRemaining = State.Weapon.BlankRemaining;

        return result;
    }

    // Moves to the next alive player
    private void AdvanceTurn()
    {
        int nextIndex = State.CurrentTurnIndex;
        int attempts = 0;

        do
        {
            nextIndex = (nextIndex + 1) % State.Players.Count;
            attempts++;
        }
        while (!State.Players[nextIndex].IsAlive && attempts <= State.Players.Count);

        State.CurrentTurnIndex = nextIndex;
    }

    // Checks if only one player remains
    private void CheckWinCondition()
    {
        var alive = State.AlivePlayers;
        if (alive.Count <= 1)
        {
            State.Phase = GamePhase.GameOver;
            if (alive.Count == 1)
                Debug.Log($"{alive[0].Name} wins the game!");
            else
                Debug.Log("No survivors. Game over.");
        }
    }
}
