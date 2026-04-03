using Godot;

public partial class PlayerStatsComponent : Node
{
    [Export]
    public PlayerStatsConfig Config { get; set; } = null!;

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int Coins { get; private set; }

    public override void _Ready()
    {
        if (Config == null)
        {
            Config = new PlayerStatsConfig();
        }

        ResetRuntimeState();
    }

    public void ResetRuntimeState()
    {
        MaxHealth = Mathf.Max(1, Config.BaseMaxHealth);
        CurrentHealth = MaxHealth;
        Coins = Mathf.Max(0, Config.StartingCoins);
    }

    public bool TakeDamage(int amount)
    {
        if (CurrentHealth <= 0)
        {
            return true;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        return CurrentHealth == 0;
    }

    public void Heal(int amount)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
    }
}
