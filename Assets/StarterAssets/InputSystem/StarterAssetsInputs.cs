using UnityEngine;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
        public bool dance;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
        public event Action OnInputChanged;


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
        public void OnDance(InputValue value)
        {
            DanceInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
            OnInputChanged?.Invoke();
        }

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
            OnInputChanged?.Invoke();
        }

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
            OnInputChanged?.Invoke();
        }

        public void DanceInput(bool newDanceState)
        {
            dance = !dance;
            OnInputChanged?.Invoke();
        }

        public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
            OnInputChanged?.Invoke();
        }

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}