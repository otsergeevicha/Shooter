using PlayerLogic;
using Plugins.MonoCache;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace InputSystem
{
	[RequireComponent(typeof(WeaponSelector))]
	[RequireComponent(typeof(Player))]
	public class StarterAssetsInputs : MonoCache
	{
		private WeaponSelector _weaponSelector;
		
		[Header("Character Input Values")]
		public Vector2 Move;
		public Vector2 Look;
		public bool Jump;
		public bool Sprint;
		public bool Aim;
		public bool Shoot;
		public bool Kick;
		public bool Uzi;
		public bool Pistol;
		public bool Grenade;
		public bool Fist;

		[Header("Movement Settings")]
		public bool AnalogMovement;

		[Header("Mouse Cursor Settings")]
		public bool CursorLocked = true;
		public bool CursorInputForLook = true;

		private void Awake()
		{
			_weaponSelector = Get<WeaponSelector>();
		}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value) => 
			MoveInput(value.Get<Vector2>());

		public void OnLook(InputValue value)
		{
			if(CursorInputForLook) 
				LookInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value) => 
			JumpInput(value.isPressed);

		public void OnSprint(InputValue value) => 
			SprintInput(value.isPressed);

		public void OnAim(InputValue value) => 
			AimInput(value.isPressed);

		public void OnShoot(InputValue value) => 
			ShootInput(value.isPressed);
		
		public void OnFist(InputValue value) => 
			FistInput(value.isPressed);

		public void OnPistol(InputValue value) => 
			PistolInput(value.isPressed);

		public void OnUzi(InputValue value) => 
			UziInput(value.isPressed);

		public void OnGrenade(InputValue value) => 
			GrenadeInput(value.isPressed);
#endif
		public void MoveInput(Vector2 newMoveDirection) => 
			Move = newMoveDirection;

		public void LookInput(Vector2 newLookDirection) => 
			Look = newLookDirection;

		public void JumpInput(bool newJumpState) => 
			Jump = newJumpState;

		public void SprintInput(bool newSprintState) => 
			Sprint = newSprintState;

		public void AimInput(bool newAimState) => 
			Aim = newAimState;

		public void ShootInput(bool newShootState) => 
			Shoot = newShootState;
		
		public void KickInput(bool virtualKickState) => 
			Kick = virtualKickState;

		public void UziInput(bool newUziState)
		{
			_weaponSelector.SelectAbility((int)IndexAbility.Uzi);
			Uzi = newUziState;
		}

		public void PistolInput(bool newPistolState)
		{
			_weaponSelector.SelectAbility((int)IndexAbility.Pistol);
			Pistol = newPistolState;
		}

		public void FistInput(bool virtualFistState)
		{
			_weaponSelector.SelectAbility((int)IndexAbility.Fist);
			Fist = virtualFistState;
		}

		public void GrenadeInput(bool newGrenadeState)
		{
			_weaponSelector.SelectAbility((int)IndexAbility.Grenade);
			Grenade = newGrenadeState;
		}

		private void OnApplicationFocus(bool hasFocus) => 
			SetCursorState(CursorLocked);

		private void SetCursorState(bool newState) => 
			Cursor.lockState = newState 
				? CursorLockMode.Locked 
				: CursorLockMode.None;
	}
}