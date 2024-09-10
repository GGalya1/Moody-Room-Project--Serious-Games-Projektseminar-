public class PlayerInMenuState : PlayerBaseState
{
    public PlayerInMenuState(PlayerStateMachine currentContext, PlayerStateFactory factory) : base(currentContext, factory)
    { }

    public override void CheckSwitchState()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
