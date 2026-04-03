using Godot;

[GlobalClass]
public partial class PlayerStatsConfig : Resource
{
    [Export(PropertyHint.Range, "1,999,1")]
    public int BaseMaxHealth { get; set; } = 10;

    [Export(PropertyHint.Range, "0,999999,1")]
    public int StartingCoins { get; set; } = 0;
}
