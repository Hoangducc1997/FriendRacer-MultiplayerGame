using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private InputActionReference _shootInput;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firingPoint;
    [SerializeField] private float _shootingForce;
    [SerializeField] private float _coolDown;

    private float _lastShotTime;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _lastShotTime >= _coolDown && _shootInput.action.IsPressed())
        {
            ShootBullet();
            _lastShotTime = Time.time;
        }
    }

    private void ShootBullet()
    {
        var bullet = Instantiate(_bulletPrefab, _firingPoint.position, _firingPoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = _firingPoint.forward * _shootingForce;
    }
}
