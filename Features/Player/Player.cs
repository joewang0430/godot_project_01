using Godot;

public partial class Player : CharacterBody2D
{
    private const string ActionMoveLeft = "move_left";
    private const string ActionMoveRight = "move_right";
    private const string ActionJump = "move_up"; // 我们借用原来的 move_up 键作为跳跃键

    [Export(PropertyHint.Range, "0,2000,1,suffix:px/s")]
    public float Speed { get; set; } = 300.0f;

    // 起跳时的初始瞬间爆发速度（因为 Y 轴向下为正，所以向上跳是负数）
    [Export]
    public float JumpVelocity { get; set; } = -400.0f;

    // 获取引擎底层物理服务器配置的标准重力加速度 (通常是 980 px/s^2)
    public float Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
    {
        // 1. 把 C++ 堆里的 Velocity 抓取到 C# 栈(Stack)上，作为一个局部 struct 变量来做高频计算
        Vector2 velocity = Velocity;

        // 2. 状态机判断：如果不接触地面 (IsOnFloor() == false)，则每一帧在 Y 轴叠加重力 (v = v0 + at)
        if (!IsOnFloor())
        {
            velocity.Y += Gravity * (float)delta;
        }

        // 3. 状态机判断：如果在地面上，并且刚按下了跳跃键，则瞬间赋予向上的 Y 轴负初速度
        if (Input.IsActionJustPressed(ActionJump) && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // 4. 水平位移处理：只取左右的输入 (-1, 0 或 1)
        float direction = Input.GetAxis(ActionMoveLeft, ActionMoveRight);
        if (direction != 0)
        {
            velocity.X = direction * Speed;
        }
        else
        {
            // 当没有输入时，利用平滑函数让水平速度瞬间或平滑归零 (模拟地面摩擦力)
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // 5. 计算完毕，跨界写回 C++ 堆内存
        Velocity = velocity;
        
        // 6. 调用 C++ 内部的物理碰撞演算。它会自动处理撞墙、挡住掉落，以及算好 IsOnFloor() 的下一帧布尔值
        MoveAndSlide();
    }
}