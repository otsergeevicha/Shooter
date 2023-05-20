using StarterAssets;
using UnityEngine;

namespace Player.InputPlayer.CanvasInputs
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection) => 
            starterAssetsInputs.MoveInput(virtualMoveDirection);

        public void VirtualLookInput(Vector2 virtualLookDirection) => 
            starterAssetsInputs.LookInput(virtualLookDirection);

        public void VirtualJumpInput(bool virtualJumpState) => 
            starterAssetsInputs.JumpInput(virtualJumpState);

        public void VirtualSprintInput(bool virtualSprintState) => 
            starterAssetsInputs.SprintInput(virtualSprintState);


        public void VirtualAimInput(bool virtualAimState) => 
            starterAssetsInputs.AimInput(virtualAimState);

        public void VirtualShootInput(bool virtualShootState) => 
            starterAssetsInputs.ShootInput(virtualShootState);
        
        public void VirtualKickInput(bool virtualKickState) => 
            starterAssetsInputs.KickInput(virtualKickState);

        public void VirtualUziInput(bool virtualUziState) => 
            starterAssetsInputs.UziInput(virtualUziState);

        public void VirtualPistolInput(bool virtualPistolState) => 
            starterAssetsInputs.PistolInput(virtualPistolState);
        
        public void VirtualGrenadeInput(bool virtualGrenadeState) => 
            starterAssetsInputs.GrenadeInput(virtualGrenadeState);
        
        public void VirtualFistInput(bool virtualFistState) => 
            starterAssetsInputs.FistInput(virtualFistState);
    }
}