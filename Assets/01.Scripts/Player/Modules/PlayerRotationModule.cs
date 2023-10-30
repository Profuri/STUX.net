using ModuleSystem;
using UnityEngine;

public class PlayerRotationModule : BaseModule<PlayerController>
{
    public override void UpdateModule()
    {
        Rotate();
    }

    public override void FixedUpdateModule()
    {
        // Do Nothing
    }

    private void Rotate()
    {
        Vector3 rotateDir;
        
        rotateDir = Controller.GetModule<PlayerMovementModule>().InputDir;

        if (rotateDir != Vector3.zero)
        {
            var rotateQuaternion = Quaternion.LookRotation(rotateDir);
            var lerpQuaternion = Quaternion.Slerp(Controller.ModelTrm.rotation, rotateQuaternion, Controller.DataSO.rotationSpeed * Time.deltaTime);
            Controller.ModelTrm.rotation = lerpQuaternion;
        }
    }
}