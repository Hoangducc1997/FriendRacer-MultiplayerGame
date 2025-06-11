using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : NetworkBehaviour, ISpawned
{
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _minAngle;
     [SerializeField] private float _maxAngle;

    private float verticalAngle;

    public override void Spawned() => _playerCamera.SetActive(HasStateAuthority);

    private void Update()
    {
        var lookValues = _lookAction.action.ReadValue<Vector2>();
        var yDelta = -lookValues.y;
        verticalAngle += yDelta * _rotateSpeed;

        verticalAngle = Mathf.Clamp(verticalAngle, _minAngle, _maxAngle);

        transform.localEulerAngles = new Vector3(verticalAngle, 0, 0);
    }
}
