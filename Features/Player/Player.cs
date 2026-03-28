using Godot;

public partial class Player : CharacterBody2D
{
    private const string ActionMoveLeft = "move_left";
    private const string ActionMoveRight = "move_right";
    private const string ActionMoveUp = "move_up";
    private const string ActionMoveDown = "move_down";

    [Export(PropertyHint.Range, "0,2000,1,suffix:px/s")]
    public float Speed { get; set; } = 240.0f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDirection = Input.GetVector(
            ActionMoveLeft,
            ActionMoveRight,
            ActionMoveUp,
            ActionMoveDown
        );

        Velocity = inputDirection * Speed;
        MoveAndSlide();
    }
}