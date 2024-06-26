using AxisConvertSystem;
using UnityEngine;

public class PlayerOnAirState : PlayerBaseState
{
    public PlayerOnAirState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        
    }
    public override void EnterState()
    {
        base.EnterState();
        InputManager.Instance.PlayerInputReader.OnAxisControlEvent += AxisControlHandle;
    }

    public override void UpdateState()
    {
        if (Player.OnGround)
        {
            SoundManager.Instance.PlaySFX("PlayerAfterJump", false, Player.SoundEffectPlayer);
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }
        
        var movementInput = InputManager.Instance.PlayerInputReader.movementInput;
        
        var dir = Quaternion.Euler(0, CameraManager.Instance.ActiveVCam.transform.eulerAngles.y, 0) * movementInput;
        if (Player.Converter.AxisType != AxisType.None)
        {
            dir.SetAxisElement(Player.Converter.AxisType, 0);
        }
        
        if (dir.sqrMagnitude > 0.05f)
        {
            Player.Rotate(Quaternion.LookRotation(dir), Player.Converter.AxisType is AxisType.None or AxisType.Y ? Player.Data.rotationSpeed : 1f);
        }

        Player.SetVelocity(dir * Player.Data.walkSpeed);
    }
    
    public override void ExitState()
    {
        base.ExitState();
        InputManager.Instance.PlayerInputReader.OnAxisControlEvent -= AxisControlHandle;
    }
}