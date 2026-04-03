using Godot;

public partial class PickupItem : Area2D
{
    public enum PickupKind
    {
        Coin = 0,
        Heart = 1,
    }

    [Export]
    public PickupKind Kind { get; set; } = PickupKind.Coin;

    [Export(PropertyHint.Range, "1,999,1")]
    public int Value { get; set; } = 1;

    [Export(PropertyHint.Range, "4,64,1,suffix:px")]
    public float Radius { get; set; } = 10.0f;

    [Export]
    public Texture2D CoinTexture { get; set; } = null!;

    [Export]
    public Texture2D HeartTexture { get; set; } = null!;

    [Export(PropertyHint.Range, "0.1,8.0,0.1")]
    public float SpriteScale { get; set; } = 1.0f;

    private bool _collected;
    private Sprite2D _visual = null!;

    public override void _Ready()
    {
        _visual = GetNodeOrNull<Sprite2D>("Visual")!;
        BodyEntered += OnBodyEntered;
        UpdateVisual();
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_visual != null && _visual.Texture != null)
        {
            return;
        }

        Color fill = Kind == PickupKind.Coin
            ? new Color(1.0f, 0.82f, 0.15f, 1.0f)
            : new Color(1.0f, 0.35f, 0.42f, 1.0f);

        DrawCircle(Vector2.Zero, Radius, fill);
    }

    private void UpdateVisual()
    {
        if (_visual == null)
        {
            return;
        }

        _visual.Texture = Kind == PickupKind.Coin ? CoinTexture : HeartTexture;
        _visual.Scale = Vector2.One * SpriteScale;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_collected || body is not Player player)
        {
            return;
        }

        _collected = true;

        switch (Kind)
        {
            case PickupKind.Coin:
                player.AddCoins(Value);
                break;
            case PickupKind.Heart:
                player.Heal(Value);
                break;
        }

        QueueFree();
    }
}
