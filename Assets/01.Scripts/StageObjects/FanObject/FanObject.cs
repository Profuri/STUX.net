using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Color = UnityEngine.Color;

public class FanObject : InteractableObject
{
    [SerializeField] private Transform _fanTrm;
    [SerializeField] private AirMaxHeightController _maxHeightController;
    [SerializeField] private ParticleSystem _airParticle;
    
    [Header("Air Settings")]
    [SerializeField] private float _airMaxHeight;
    [SerializeField] private float _airPower;
    
    [Header("Fan Settings")]
    [SerializeField] private float _fanMaxSpeed;
    [SerializeField] private float _fanAccelerator;
    [SerializeField] private float _fanDecelerator;

    private float _currentFanSpeed;

    private bool _enabled;
    public bool Enabled => _enabled;

    private void Update()
    {
        RotateFan();
        
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            EnableFan();
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ReleaseFan();
        }
    }

    private void EnableFan()
    {
        _enabled = true;
        _airParticle.Play();
    }

    private void ReleaseFan()
    {
        _enabled = false;
        _airParticle.Stop();
        _maxHeightController.StopParticle();
    }
    
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        _enabled = interactValue;
        if (_enabled)
        {
            EnableFan();
        }
        else
        {
            ReleaseFan();
        }
    }

    private void RotateFan()
    {
        _currentFanSpeed += (_enabled ? _fanAccelerator : -_fanDecelerator) * Time.deltaTime;
        _currentFanSpeed = Mathf.Clamp(_currentFanSpeed, 0, _fanMaxSpeed);
        _fanTrm.Rotate(new Vector3(0, _currentFanSpeed * Time.deltaTime, 0));
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (_maxHeightController == null)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_maxHeightController.GetCenterPos(), _maxHeightController.GetColSize());
    }
    
    private void OnValidate()
    {
        if (_maxHeightController == null)
        {
            return;
        }
        _maxHeightController.SetHeight(_airMaxHeight);
    }

#endif
}
