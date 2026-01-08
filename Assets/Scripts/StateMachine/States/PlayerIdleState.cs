using StateMachine;
using UnityEngine;

public class PlayerIdleState : PlayerState 
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        //enable walking keybind
        //enable attack keybind
        //enable dodge keybind
        //enable input for run
    }
}
