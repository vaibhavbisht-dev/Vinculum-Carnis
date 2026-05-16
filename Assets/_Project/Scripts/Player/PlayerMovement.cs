using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _orientation;
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _sprintAction;
    [SerializeField] private InputActionReference _crouchAction;


    [Header("Player Movement Settings")]
    [SerializeField] private float _speed = 7f;
    [SerializeField] private float _jumpHeight = 2.5f;
    [SerializeField] private float _sprintSpeed = 10f;

    [Header("Gravity")]
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;

    public enum PlayerState { Alive, Dead}

    Vector3 Velocity;

    public PlayerState CurrentState { get; private set; } = PlayerState.Alive;


    bool _isGrounded = false;


    private void Start()
    {
        _moveAction.action.Enable();
        _jumpAction.action.Enable();
        _sprintAction.action.Enable();
        _crouchAction.action.Enable();
    }

    private void OnDestroy()
    {

    }


    private void Update()
    {
        CheckGround();
        Movement();
        Jump();
        ApplyGravity();
    }

    private void Movement()
    {
        Vector2 input = _moveAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = _orientation.forward * input.y + _orientation.right * input.x;
        
        moveDirection.Normalize();
        
        float currentSpeed = _speed;
        if (_sprintAction.action.IsPressed() && _isGrounded)
        {
            currentSpeed = _sprintSpeed;
        }
        _characterController.Move(moveDirection  * currentSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (_jumpAction.action.WasPressedThisDynamicUpdate() && _isGrounded)
        {
            Debug.Log("Jump pressed");
            Velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }
    }

    private void CheckGround() { 
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
    }

    private void ApplyGravity() {
        if (_isGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }

        Velocity.y += _gravity * Time.deltaTime;


        _characterController.Move(Velocity * Time.deltaTime);
    }




}
