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
                   _input.PlayerActions.Move.started += OnMovePerformed;
                   _input.PlayerActions.Move.performed += OnMovePerformed;
                   _input.PlayerActions.Move.canceled += OnMoveCanceled;
                   Debug.Log("MoveSubbed");
               return;
               case false:
                   //un sub from walk logic
                   _input.PlayerActions.Move.started -= OnMovePerformed;
                   _input.PlayerActions.Move.performed -= OnMovePerformed;
                   _input.PlayerActions.Move.canceled -= OnMoveCanceled;
               return;
           }
       }
       
       private void OnMovePerformed(InputAction.CallbackContext context)
       {
           _stateMachine.ChangeState(_stateMachine.WalkState);
           _stateMachine.Move(context.ReadValue<Vector2>());
           Debug.Log(context);
       }
       
       private void OnMoveCanceled(InputAction.CallbackContext context)
       {
           _stateMachine.Move(Vector2.zero);
           Debug.Log("MoveCanceled");
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
                   _input.PlayerActions.Jump.performed += OnJumpPerformed;
                   return;
               case false:
                   //un sub from Jump logic
                   _input.PlayerActions.Jump.performed -= OnJumpPerformed;
                   return;
           }
       }

       private void OnJumpPerformed(InputAction.CallbackContext context)
       {
           Debug.Log("Jump input detected!");
           _stateMachine.Jump();
           Debug.Log(context);
       }
       
       private void DodgeInputs(bool enable)
       {
           switch (enable)
           {
               case true:
                   _input.PlayerActions.Dodge.performed += OnDodgePerformed;
                   // sub to Dodge logic 
                   return;
               case false:
                   //un sub from Dodge logic
                   _input.PlayerActions.Dodge.performed -= OnDodgePerformed;
                   return;
           }
       }

       private void OnDodgePerformed(InputAction.CallbackContext context)
       {
           _stateMachine.Dodge();
           Debug.Log(context);
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
           _input.PlayerActions.Enable();
           WalkInputs(true);
           SprintInputs(true);
           InteractInputs(true);
           DodgeInputs(true);
           JumpInputs(true);
           AttackInputs(true);
       }
    }
}
