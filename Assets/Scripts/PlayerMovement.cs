using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour, ISpawned
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private NetworkCharacterController _networkController;
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Animator _anim;

    [SerializeField] private InputActionReference _lookInput;

    [Networked, OnChangedRender(nameof(OnVelocityChanged))]
    private Vector3 Velocity { get; set;}

    private void OnVelocityChanged()
    {
        _anim.SetFloat("Speed", Velocity.magnitude);
    }

    public override void Spawned()
    {
        base.Spawned();
        if (!HasStateAuthority) { return; }

        _characterController.enabled = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        
        if (!HasStateAuthority) { return; }
        UpdateMovement();
        _networkController.Move(Velocity); 
        //UpdateRotation();
    }

    private void UpdateMovement()
    {
        var direction = _speed * GetMoveDirection();
        Velocity = direction;
    }

    private Vector3 GetMoveDirection()
    {
        var inputValues = _moveInput.action.ReadValue<Vector2>();
        var direction = transform.forward * inputValues.y
            + transform.right * inputValues.x;
        return direction.normalized;
    }

    private void UpdateRotation()
    {
        var lookValues = _lookInput.action.ReadValue<Vector2>();
        transform.Rotate(0, lookValues.x * _rotateSpeed * Runner.DeltaTime, 0);
    }
}
