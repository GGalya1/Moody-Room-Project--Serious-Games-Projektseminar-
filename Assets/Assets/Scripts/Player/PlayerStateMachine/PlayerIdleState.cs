//macht es aber Sinn?
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory factory): base(currentContext, factory)
    {}

    public override void CheckSwitchState()
    {
        //if idle (grounded) and press "jump", then go to the jump state
        if (Input.GetButtonDown("Jump"))
        {
            SwitchState(_factory.Jump());
        }
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
