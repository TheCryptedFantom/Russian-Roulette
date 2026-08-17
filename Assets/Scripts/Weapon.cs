using System;
using System.Collections.Generic;

// ============================================================
// WEAPON / SHELL SYSTEM (live/blank rounds, Buckshot-Roulette style)
// ============================================================
public class Weapon
{
    // The current shuffled queue of shells. true = live, false = blank.
    public List<bool> Shells { get; private set; } = new List<bool>();
    public int CurrentIndex { get; private set; } = 0;

    private int _liveCount;
    private int _blankCount;
    private System.Random _rng = new System.Random();

    public Weapon(int liveCount = 2, int blankCount = 4)
    {
        LoadShells(liveCount, blankCount);
    }

    // Builds a new shuffled set of shells. Call this to "reload" between rounds
    // or automatically when the current set runs out.
    public void LoadShells(int liveCount, int blankCount)
    {
        _liveCount = liveCount;
        _blankCount = blankCount;

        Shells = new List<bool>();
        for (int i = 0; i < liveCount; i++) Shells.Add(true);
        for (int i = 0; i < blankCount; i++) Shells.Add(false);

        Shuffle(Shells);
        CurrentIndex = 0;
    }

    // Pulls the trigger on the next shell in the queue.
    // Returns true if it's a live round, false if it's a blank.
    // Auto-reloads with the same live/blank ratio if the queue is empty.
    public bool PullTrigger()
    {
        if (CurrentIndex >= Shells.Count)
        {
            LoadShells(_liveCount, _blankCount);
        }

        bool isLive = Shells[CurrentIndex];
        CurrentIndex++;
        return isLive;
    }

    // How many shells are left before an auto-reload happens
    public int ShellsRemaining => Shells.Count - CurrentIndex;

    // Breakdown of what's left in the current queue (for UI display)
    public int LiveRemaining
    {
        get
        {
            int count = 0;
            for (int i = CurrentIndex; i < Shells.Count; i++)
                if (Shells[i]) count++;
            return count;
        }
    }

    public int BlankRemaining
    {
        get
        {
            int count = 0;
            for (int i = CurrentIndex; i < Shells.Count; i++)
                if (!Shells[i]) count++;
            return count;
        }
    }

    private void Shuffle(List<bool> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
