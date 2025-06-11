using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIcon : NetworkBehaviour
{
    [SerializeField] private GameObject _icon;

    [Networked, OnChangedRender(nameof(IconChanged))]
    public bool NetworkedIsIconActive { get; set; }

    private void Update()
    {
        if (HasStateAuthority && Keyboard.current.iKey.wasPressedThisFrame)
        {
            NetworkedIsIconActive = true;
        }
        if (HasStateAuthority && Keyboard.current.iKey.wasReleasedThisFrame)
        {
            NetworkedIsIconActive = false;
        }
    }

    private void IconChanged() => _icon.SetActive(NetworkedIsIconActive);
}
