using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DressController), typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : NetworkBehaviour
{
    public static PlayerController prov;

    public enum Pose { Idle, WalkLeftFoot, WalkRightFoot };

    public delegate void _OnPoseChanged(Pose pose, bool flipX);
    public event _OnPoseChanged OnPoseChanged;

    [SerializeField, HideInInspector] private DressController _dressController;
    [SerializeField, HideInInspector] private PlayerInput _input;
    [SerializeField, HideInInspector] private Rigidbody2D _rb;

    [SerializeField] private float _speed = 5f;

    [SerializeField] private float _animationSwapTime = 0.2f;
    [SerializeField, Range(0.01f, 2f)] private float _animationThresholdDifference = 0.1f;
    private float _nextSwapAt;
    private int _animationPhase = 0;
    private bool _flipX = false;

    Vector2 _movement = new();
    Vector3 _lastPosition = new();
    bool _inputActive = false;

    void OnValidate()
    {
        _dressController = GetComponent<DressController>();
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        prov = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner && !_inputActive)
        {
            _inputActive = true;

            _input.ActivateInput();
            _input.actions["Move"].performed += Move;
            _input.actions["Move"].canceled += Move;
        }
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.NetworkTickSystem.Tick += MovementAnimation;

        if (IsOwner && !_inputActive)
        {
            _inputActive = true;

            _input.ActivateInput();
            _input.actions["Move"].performed += Move;
            _input.actions["Move"].canceled += Move;
        }
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.NetworkTickSystem.Tick -= MovementAnimation;


        if (IsOwner && _inputActive)
        {
            _inputActive = false;

            _input.DeactivateInput();
            _input.actions["Move"].performed -= Move;
            _input.actions["Move"].canceled -= Move;
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _speed * Time.deltaTime * _movement;
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _movement = ctx.ReadValue<Vector2>();

            // if(_movement.x != 0) _dressController.FlipX(_movement.x > 0);
        }
        else if (ctx.canceled)
        {
            _movement = Vector3.zero;
        }

    }

    private void MovementAnimation()
    {
        if (Vector2.Distance(_lastPosition, transform.position) > _animationThresholdDifference)
        {
            // If the player is moving and the animation has to be changed
            if (Time.time > _nextSwapAt || _nextSwapAt == float.PositiveInfinity)
            {
                _flipX = _lastPosition.x - transform.position.x < 0;

                switch (_animationPhase)
                {
                    case 0:
                        OnPoseChanged?.Invoke(Pose.WalkLeftFoot, _flipX);
                        break;
                    case 1:
                        OnPoseChanged?.Invoke(Pose.Idle, _flipX);
                        break;
                    case 2:
                        OnPoseChanged?.Invoke(Pose.WalkRightFoot, _flipX);
                        break;
                    case 3:
                        OnPoseChanged?.Invoke(Pose.Idle, _flipX);
                        break;
                }

                _nextSwapAt = Time.time + _animationSwapTime;
                _animationPhase = (_animationPhase + 1) % 4;
            }
        }
        else
        {
            OnPoseChanged?.Invoke(Pose.Idle, _flipX);

            _nextSwapAt = float.PositiveInfinity;
            _animationPhase = 0;
        }

        _lastPosition = transform.position;
    }

}
