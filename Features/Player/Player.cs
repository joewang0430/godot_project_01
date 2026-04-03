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

    [Export]
    public Node2D GunPivot { get; set; } = null!;

    [Export]
    public Marker2D Muzzle { get; set; } = null!;

    [Export]
    public PackedScene BulletScene { get; set; } = null!;

    [Export(PropertyHint.Range, "0.05,2.0,0.01,suffix:s")]
    public float FireInterval { get; set; } = 0.2f;

    [Export]
    public PlayerStatsComponent Stats { get; set; } = null!;

    private float _fireCooldown;
    private Node _currentScene = null!;
    public int CurrentHealth => Stats?.CurrentHealth ?? 0;
    public int MaxHealth => Stats?.MaxHealth ?? 0;
    public int Coins => Stats?.Coins ?? 0;
    public bool IsDead => CurrentHealth <= 0;

    // 获取引擎底层物理服务器配置的标准重力加速度 (通常是 980 px/s^2)
    public float Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        _currentScene = GetTree().CurrentScene;

        if (Stats == null)
        {
            Stats = GetNodeOrNull<PlayerStatsComponent>("Stats");
        }

        if (Stats == null)
        {
            GD.PushWarning("Player Stats component missing. Creating fallback stats component.");
            Stats = new PlayerStatsComponent();
            Stats.Name = "Stats";
            AddChild(Stats);
        }

        Stats.ResetRuntimeState();

        _stateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, _stateMachine);
        RunState = new PlayerRunState(this, _stateMachine);
        AirState = new PlayerAirState(this, _stateMachine);

        _stateMachine.Initialize(IsOnFloor() ? IdleState : AirState);
    }

    public override void _PhysicsProcess(double delta)
    {
        _fireCooldown = Mathf.Max(0.0f, _fireCooldown - (float)delta);

        Vector2 velocity = Velocity;
        bool isOnFloor = IsOnFloor();
        bool jumpPressed = Input.IsActionJustPressed(ActionJump);
        float direction = Input.GetAxis(ActionMoveLeft, ActionMoveRight);

        _stateMachine.PhysicsUpdate(delta, isOnFloor, jumpPressed, direction, ref velocity);

        Velocity = velocity;
        MoveAndSlide();

        TryShoot();
    }

    public override void _Process(double delta)
    {
        AimGunAtMouse();
    }

    private void AimGunAtMouse()
    {
        if (GunPivot == null)
        {
            return;
        }

        GunPivot.LookAt(GetGlobalMousePosition());
    }

    private void TryShoot()
    {
        if (!Input.IsMouseButtonPressed(MouseButton.Left))
        {
            return;
        }

        if (_fireCooldown > 0.0f || BulletScene == null || Muzzle == null)
        {
            return;
        }

        Bullet bullet = BulletScene.Instantiate<Bullet>();
        _currentScene.AddChild(bullet);
        bullet.GlobalPosition = Muzzle.GlobalPosition;
        bullet.GlobalRotation = Muzzle.GlobalRotation;
        bullet.SetDirection(Muzzle.GlobalTransform.X.Normalized());

        _fireCooldown = FireInterval;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0 || Stats == null)
        {
            return;
        }

        bool died = Stats.TakeDamage(amount);
        GD.Print($"Player HP: {CurrentHealth}/{MaxHealth}");

        if (died)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0 || Stats == null)
        {
            return;
        }

        int before = CurrentHealth;
        Stats.Heal(amount);
        if (CurrentHealth != before)
        {
            GD.Print($"Player HP: {CurrentHealth}/{MaxHealth}");
        }
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0 || Stats == null)
        {
            return;
        }

        Stats.AddCoins(amount);
        GD.Print($"Coins: {Coins}");
    }

    private void Die()
    {
        GD.Print("Player died.");
        QueueFree();
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