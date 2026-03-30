using Godot;

public sealed class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
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
        if (!Mathf.IsZeroApprox(direction))
        {
            StateMachine.ChangeState(Player.RunState);
        }
    }
}
