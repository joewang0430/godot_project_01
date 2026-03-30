using Godot;

public partial class Player : CharacterBody2D
{
    private const string ActionMoveLeft = "move_left";
    private const string ActionMoveRight = "move_right";
    private const string ActionJump = "move_up"; // 我们借用原来的 move_up 键作为跳跃键

    private PlayerStateMachine _stateMachine = null!;

    internal PlayerIdleState IdleState { get; private set; } = null!;
    internal PlayerRunState RunState { get; private set; } = null!;
    internal PlayerAirState AirState { get; private set; } = null!;

    [Export(PropertyHint.Range, "0,2000,1,suffix:px/s")]
    public float Speed { get; set; } = 300.0f;

    // 起跳时的初始瞬间爆发速度（因为 Y 轴向下为正，所以向上跳是负数）
    [Export]
    public float JumpVelocity { get; set; } = -400.0f;

    // 获取引擎底层物理服务器配置的标准重力加速度 (通常是 980 px/s^2)
    public float Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        _stateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, _stateMachine);
        RunState = new PlayerRunState(this, _stateMachine);
        AirState = new PlayerAirState(this, _stateMachine);

        _stateMachine.Initialize(IsOnFloor() ? IdleState : AirState);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        bool isOnFloor = IsOnFloor();
        bool jumpPressed = Input.IsActionJustPressed(ActionJump);
        float direction = Input.GetAxis(ActionMoveLeft, ActionMoveRight);

        _stateMachine.PhysicsUpdate(delta, isOnFloor, jumpPressed, direction, ref velocity);

        Velocity = velocity;
        MoveAndSlide();
    }

    internal void ApplyHorizontal(float direction, ref Vector2 velocity)
    {
        if (!Mathf.IsZeroApprox(direction))
        {
            velocity.X = direction * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
        }
    }

    internal void ApplyGravity(float delta, ref Vector2 velocity)
    {
        velocity.Y += Gravity * delta;
    }

    internal void ApplyJump(ref Vector2 velocity)
    {
        velocity.Y = JumpVelocity;
    }
}