using Godot;

public abstract class PlayerState
{
    protected PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        Player = player;
        StateMachine = stateMachine;
    }

    protected Player Player { get; }
    protected PlayerStateMachine StateMachine { get; }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public abstract void PhysicsUpdate(double delta, bool isOnFloor, bool jumpPressed, float direction, ref Vector2 velocity);
}
