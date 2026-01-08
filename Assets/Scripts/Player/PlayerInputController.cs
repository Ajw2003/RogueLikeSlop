using Code.Scripts.EventSystems;
using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController : MonoBehaviour
    {
       private PlayerStateMachine _stateMachine;
       private PlayerInputs _input;

       private void Awake()
       {
           if (_input == null)
           {
               _input = new  PlayerInputs();
           }
           _stateMachine = GetComponent<PlayerStateMachine>();
           EventManager.Instance?.Subscribe(this, (PlayerIdleEvent e) => EnableAllInputs());
       }

       private void WalkInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   // sub to walk logic 
                   _input.PlayerActions.Move.performed += OnMovePerformed;
                   Debug.Log("MoveSubbed");
               return;
               case false:
                   //un sub from walk logic
               return;
           }
       }

       private void SprintInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   // sub to Sprint logic 
                   return;
               case false:
                   //un sub from Sprint logic
                   return;
           }
       }

       private void InteractInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   // sub to Interact logic 
                   return;
               case false:
                   //un sub from Interact logic
                   return;
           }
       }

       private void AttackInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   // sub to attack logic 
                   _input.PlayerActions.Attack.performed += OnAttackPerformed;
                   return;
               case false:
                   //un sub from Attack logic
                   _input.PlayerActions.Attack.performed -= OnAttackPerformed;
                   return;
           }
       }

       private void OnAttackPerformed(InputAction.CallbackContext context)
       {
           EventManager.Instance?.Publish(new PlayerAttackEvent ());
       }

       private void JumpInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   // sub to Jump logic 
                   _input.PlayerActions.Move.performed += OnMovePerformed;
                   return;
               case false:
                   //un sub from Jump logic
                   return;
           }
       }

       private void OnMovePerformed(InputAction.CallbackContext context)
       {
           _stateMachine.Move(context.ReadValue<Vector2>());
           Debug.Log(context);
       }

       private void DodgeInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   // sub to Dodge logic 
                   return;
               case false:
                   //un sub from Dodge logic
                   return;
           }
       }

       private void PlayerDied()
       {
           DisableAllInputs();
       }

       private void PlayerRespawned()
       {
           EnableAllInputs();
       }
       private void DisableAllInputs()
       {
           WalkInputs(false);
           SprintInputs(false);
           InteractInputs(false);
           DodgeInputs(false);
           JumpInputs(false);
           AttackInputs(false);
       }

       private void EnableAllInputs()
       {
           WalkInputs(true);
           SprintInputs(true);
           InteractInputs(true);
           DodgeInputs(true);
           JumpInputs(true);
           AttackInputs(true);
       }
    }
}
