using System;

// ============================================================
// PLAYER
// ============================================================
[Serializable]
public class Player
{
    public string Id;
    public string Name;
    public bool IsAlive = true;
    public bool IsHuman = true; // false = AI-controlled

    public int MaxHealth = 3;
    public int Health = 3;

    public Player(string id, string name, bool isHuman, int maxHealth = 3)
    {
        Id = id;
        Name = name;
        IsHuman = isHuman;
        IsAlive = true;
        MaxHealth = maxHealth;
        Health = maxHealth;
    }
}
