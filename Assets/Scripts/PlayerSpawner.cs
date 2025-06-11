using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;

    public void PlayerJoined(PlayerRef player)
    {
        print("Player joined: " + player.PlayerId);
        if (player == Runner.LocalPlayer)
        {
            int locationIndex = (player.PlayerId - 1) % _spawnPoints.Length;
            var spawnPoint = _spawnPoints[locationIndex];
            Runner.Spawn(_playerPrefab, spawnPoint.position, spawnPoint.rotation, player);
        }
    }
}
