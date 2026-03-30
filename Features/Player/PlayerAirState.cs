using Godot;

public sealed class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void PhysicsUpdate(double delta, bool isOnFloor, bool jumpPressed, float direction, ref Vector2 velocity)
    {
        Player.ApplyGravity((float)delta, ref velocity);
        Player.ApplyHorizontal(direction, ref velocity);

        if (isOnFloor)
        {
            if (Mathf.IsZeroApprox(direction))
            {
                StateMachine.ChangeState(Player.IdleState);
            }
            else
            {
                StateMachine.ChangeState(Player.RunState);
            }
        }
    }
}
