using Godot;

public sealed class PlayerStateMachine
{
    private PlayerState _currentState = null!;

    public void Initialize(PlayerState initialState)
    {
        _currentState = initialState;
        _currentState.Enter();
    }

    public void ChangeState(PlayerState nextState)
    {
        if (_currentState == nextState)
        {
            return;
        }

        _currentState.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }

    public void PhysicsUpdate(double delta, bool isOnFloor, bool jumpPressed, float direction, ref Vector2 velocity)
    {
        _currentState.PhysicsUpdate(delta, isOnFloor, jumpPressed, direction, ref velocity);
    }
}
