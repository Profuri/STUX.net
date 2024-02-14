using AxisConvertSystem;
using InputControl;
using InteractableSystem;
using UnityEngine;

public class PlayerUnit : ObjectUnit
{
    [SerializeField] private InputReader _inputReader;
    public InputReader InputReader => _inputReader;
    
    [SerializeField] private PlayerData _data;
    public PlayerData Data => _data;

    public Transform ModelTrm { get; private set; }
    public Animator Animator { get; private set; }
    public ObjectHoldingHandler HoldingHandler { get; private set; }
    private StateController _stateController;
    private PlayerUIController _playerUiController;

    public bool OnGround => CheckGround();

    private InteractableObject _selectedInteractableObject;

    private readonly int _activeHash = Animator.StringToHash("Active");

    public override void Awake()
    {
        base.Awake();
        Converter = GetComponent<AxisConverter>();
        ModelTrm = transform.Find("Model");
        Animator = ModelTrm.GetComponent<Animator>();
        HoldingHandler = GetComponent<ObjectHoldingHandler>();
        _playerUiController = transform.Find("PlayerCanvas").GetComponent<PlayerUIController>();
        
        _stateController = new StateController(this);
        _stateController.RegisterState(new PlayerIdleState(_stateController, true, "Idle"));
        _stateController.RegisterState(new PlayerMovementState(_stateController, true, "Movement"));
        _stateController.RegisterState(new PlayerJumpState(_stateController, true, "Jump"));
        _stateController.RegisterState(new PlayerFallState(_stateController, true, "Fall"));
        _stateController.RegisterState(new PlayerAxisControlState(_stateController));
    }

    private void Update()
    {
        _stateController.UpdateState();

        _selectedInteractableObject = FindInteractable();
        _playerUiController.SetKeyGuide(_selectedInteractableObject is not null);
    }

    public override void OnPop()
    {
        _inputReader.OnInteractionEvent += OnInteraction;
        _stateController.ChangeState(typeof(PlayerIdleState));
        Animator.SetBool(_activeHash, true);
    }

    public override void OnPush()
    {
        _inputReader.OnInteractionEvent -= OnInteraction;
        Animator.SetBool(_activeHash, false);
    }

    public void SetVelocity(Vector3 velocity, bool useGravity = true)
    {
        if (useGravity)
        {
            Rigidbody.AddForce(Vector3.up * _data.gravity);
            velocity.y = Rigidbody.velocity.y;
        }
        Rigidbody.velocity = velocity;
    }

    public void StopImmediately(bool withYAxis)
    {
        Rigidbody.velocity = withYAxis ? Vector3.zero : new Vector3(0, Rigidbody.velocity.y, 0);
    }

    public void Rotate(Quaternion rot, float speed = -1)
    {
        ModelTrm.rotation = Quaternion.Lerp(ModelTrm.rotation, rot, speed < 0 ? _data.rotationSpeed : speed);
    }
    
    private bool CheckGround()
    {
        var size = Collider.bounds.size * 0.8f;
        var center = Collider.bounds.center;
        size.y = 0.1f;
        var isHit = Physics.BoxCast(
            center,
            size,
            -transform.up,
            out var hit,
            ModelTrm.rotation,
            _data.groundCheckDistance,
            _data.groundMask
        );
        
        return isHit && !hit.collider.isTrigger;
    }
    
    private InteractableObject FindInteractable()
    {
        var cols = new Collider[_data.maxInteractableCnt];
        var size = Physics.OverlapSphereNonAlloc(Collider.bounds.center, _data.interactableRadius, cols, _data.interactableMask);

        for(var i = 0; i < size; ++i)
        {
            var interactable = cols[i].GetComponent<InteractableObject>();
            
            if (interactable is null)
            {
                continue;
            }
                
            if(interactable.InteractType == EInteractType.INPUT_RECEIVE)
            {
                return interactable;
            }
        }

        return null;
    }

    public void SetSection(Section section)
    {
        transform.SetParent(section.transform);
        Converter.Init(section);
        _originUnitInfo.LocalPos = section.PlayerResetPoint;
    }

    public void AnimationTrigger(string key)
    {
        _stateController.CurrentState.AnimationTrigger(key);
    }

    private void OnInteraction()
    {
        if (_selectedInteractableObject is null)
        {
            return;
        }
        
        _selectedInteractableObject.OnInteraction(this, true);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            return;
        }
        
        Gizmos.color = Color.cyan;

        var col = GetComponent<Collider>();
        Gizmos.DrawWireSphere(col.bounds.center, _data.interactableRadius);
        
        if (_selectedInteractableObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(col.bounds.center, _selectedInteractableObject.transform.position);
        }
    }
#endif
}