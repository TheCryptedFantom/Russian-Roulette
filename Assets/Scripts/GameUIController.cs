using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // remove this + swap Text/Dropdown types if not using TextMeshPro

public class GameUIController : MonoBehaviour
{
    public TurnManager Turns;             // drag your TurnManager GameObject here in Inspector
    public TMP_Text StatusText;           // shot result log (no turn info here)
    public TMP_Text CurrentTurnText;      // dedicated "whose turn" label
    public TMP_Text ShellCountText;       // dedicated "X live / Y blank remaining" label
    public TMP_Dropdown TargetDropdown;   // drag your target-selection Dropdown here

    // Hook this to StartButton's OnClick()
    public void OnStartClicked()
    {
        var players = new List<Player>
        {
            new Player("p1", "Alice", true),
            new Player("p2", "Bob", false),
            new Player("p3", "Carol", false),
            new Player("p4", "Dave", false)
        };

        Turns.StartNewGame(players); // defaults: 2 live / 4 blank shells, 3 health each
        RefreshTargetDropdown();
        UpdateCurrentTurnText();
        UpdateShellCountText(Turns.State.Weapon.LiveRemaining, Turns.State.Weapon.BlankRemaining);
        UpdateStatusText("Game started.");
    }

    // Hook this to ShootButton's OnClick()
    public void OnShootClicked()
    {
        if (Turns.State == null || Turns.State.Phase == GamePhase.GameOver)
        {
            UpdateStatusText("Game not running. Press Start.");
            return;
        }

        Player target = GetSelectedTarget();
        if (target == null)
        {
            UpdateStatusText("No target selected.");
            return;
        }

        ShotResult result = Turns.TakeShot(target);
        UpdateStatusText(BuildResultMessage(result));
        UpdateShellCountText(result.LiveRemaining, result.BlankRemaining);

        RefreshTargetDropdown();
        UpdateCurrentTurnText();
    }

    // Builds the shot-result message: who shot who, live/blank, and damage outcome.
    // Deliberately excludes any "whose turn" info - that lives in CurrentTurnText.
    private string BuildResultMessage(ShotResult result)
    {
        if (result.Shooter == null) return "Shot could not be resolved.";

        string roundType = result.WasLive ? "LIVE" : "blank";
        string message = $"{result.Shooter.Name} shot {result.Target.Name} — {roundType}.";

        if (result.WasLive)
        {
            message += result.TargetEliminated
                ? $" {result.Target.Name} was eliminated."
                : $" {result.Target.Name} has {result.Target.Health} health left.";
        }
        else
        {
            message += result.WentAgain
                ? $" No damage. {result.Shooter.Name} goes again."
                : " No damage.";
        }

        if (Turns.State.Phase == GamePhase.GameOver)
        {
            var alive = Turns.State.AlivePlayers;
            message += alive.Count == 1 ? $" {alive[0].Name} wins!" : " Game over.";
        }

        return message;
    }

    // Rebuilds the dropdown options from the current list of alive players.
    // Explicitly resets the selection so a stale index can't point at the wrong name.
    private void RefreshTargetDropdown()
    {
        if (TargetDropdown == null || Turns.State == null) return;

        List<string> names = Turns.State.AlivePlayers.Select(p => p.Name).ToList();
        TargetDropdown.ClearOptions();
        TargetDropdown.AddOptions(names);
        TargetDropdown.value = 0;
        TargetDropdown.RefreshShownValue();
    }

    // Maps the dropdown's selected name back to the actual Player object
    private Player GetSelectedTarget()
    {
        if (TargetDropdown == null || Turns.State == null) return null;
        if (TargetDropdown.options.Count == 0) return null;

        string selectedName = TargetDropdown.options[TargetDropdown.value].text;
        return Turns.State.AlivePlayers.FirstOrDefault(p => p.Name == selectedName);
    }

    // Dedicated "whose turn" indicator, kept separate from the shot-result log
    private void UpdateCurrentTurnText()
    {
        if (CurrentTurnText == null || Turns.State == null) return;
        CurrentTurnText.text = Turns.State.Phase == GamePhase.GameOver
            ? "Game over"
            : $"Current turn: {Turns.State.CurrentPlayer.Name}";
    }

    // Dedicated "shells remaining" indicator
    private void UpdateShellCountText(int liveRemaining, int blankRemaining)
    {
        if (ShellCountText == null) return;
        ShellCountText.text = $"Chamber: {liveRemaining} live / {blankRemaining} blank remaining";
    }

    private void UpdateStatusText(string message)
    {
        if (StatusText != null)
            StatusText.text = message;
        Debug.Log(message);
    }
}
