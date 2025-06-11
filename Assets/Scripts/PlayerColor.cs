using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColor : NetworkBehaviour, ISpawned
{
    [SerializeField] private MeshRenderer _bodyRenderer;

    [Networked, OnChangedRender(nameof(ColorChanged))]
    public Color NetworkedColor { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            PickRandomColor();
        }
        else
        {
            ColorChanged();
        }
    }

    private void Update()
    {
        if (HasStateAuthority && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickRandomColor();
        }
    }

    private void PickRandomColor() => NetworkedColor = Random.ColorHSV();

    private void ColorChanged() => _bodyRenderer.material.color = NetworkedColor;
}
