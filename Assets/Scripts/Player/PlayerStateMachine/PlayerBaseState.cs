public abstract class PlayerBaseState

{
    protected PlayerStateFactory _factory;
    protected PlayerStateMachine _ctx;
    public PlayerBaseState(PlayerStateMachine context, PlayerStateFactory factory)
    {
        _ctx = context;
        _factory = factory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();

    void UpdateStates()
    {

    }
    protected void SwitchState(PlayerBaseState newState)
    {
        //verlassen aktuellen Zustand, indem alle "Verlass"-Aktionen unternohmen werden muessen
        ExitState();

        //unternehmen Aktionen, um im neuen Zustand zu kommen
        newState.EnterState();

        _ctx.CurrentState = newState;
    }
}
