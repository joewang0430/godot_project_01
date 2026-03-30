using Godot;

public sealed class PlayerRunState : PlayerState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)    // 存在了哪里？存在了这块连续内存地址的 0x1008 和 0x1010 的物理位置上。由于这两个物理位置是被父类定义的，子类不敢随便越权写，所以它打了个 base 的电话，让父类亲自跑过来把数据填进这俩坑里
    {
    }

    public override void PhysicsUpdate(double delta, bool isOnFloor, bool jumpPressed, float direction, ref Vector2 velocity)
    {
        if (!isOnFloor)
        {
            StateMachine.ChangeState(Player.AirState);
            Player.AirState.PhysicsUpdate(delta, isOnFloor, jumpPressed, direction, ref velocity);
            return;
        }

        if (jumpPressed)
        {
            Player.ApplyJump(ref velocity);
            StateMachine.ChangeState(Player.AirState);
            return;
        }

        Player.ApplyHorizontal(direction, ref velocity);
        if (Mathf.IsZeroApprox(direction))
        {
            StateMachine.ChangeState(Player.IdleState);
        }
    }
}
