using Godot;

public partial class Bullet : Area2D
{
    [Export(PropertyHint.Range, "10,3000,1,suffix:px/s")]
    public float Speed { get; set; } = 900.0f;

    [Export(PropertyHint.Range, "0.1,5.0,0.1,suffix:s")]
    public float Lifetime { get; set; } = 1.5f;

    [Export(PropertyHint.Range, "1,32,1,suffix:px")]
    public float VisualRadius { get; set; } = 5.0f;

    [Export]
    public Color VisualColor { get; set; } = new Color(1.0f, 0.9f, 0.2f, 1.0f);

    private Vector2 _direction = Vector2.Right;
    private float _timeLeft;

    public override void _Ready()
    {
        _timeLeft = Lifetime;
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, VisualRadius, VisualColor);
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += _direction * Speed * (float)delta;

        _timeLeft -= (float)delta;
        if (_timeLeft <= 0.0f)
        {
            QueueFree();
        }
    }

    public void SetDirection(Vector2 direction)
    {
        if (!direction.IsZeroApprox())
        {
            _direction = direction.Normalized();
        }
    }
}
