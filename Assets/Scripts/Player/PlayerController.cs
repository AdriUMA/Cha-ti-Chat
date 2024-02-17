using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(Collider))]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, HideInInspector] private PlayerInput _input;
    [SerializeField, HideInInspector] private Rigidbody2D _rb;
    [SerializeField, HideInInspector] private Animator _animator;
    [SerializeField, HideInInspector] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _speed = 5f;

    Vector2 _movement = new();

    void OnValidate()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        _input.ActivateInput();
        _input.actions["Move"].performed += Move;
        _input.actions["Move"].canceled += Move;
    }

    private void OnDisable()
    {
        _input.DeactivateInput();
        _input.actions["Move"].performed -= Move;
        _input.actions["Move"].canceled -= Move;
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

            _spriteRenderer.flipX = _movement.x > 0;
            _animator.SetBool("Moving", true);
        }
        else if (ctx.canceled)
        {
            _movement = Vector3.zero;
            _animator.SetBool("Moving", false);
        }
    }
}
