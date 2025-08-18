using System;
using Main.Scripts.Player;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponent<PlayerManager>();
        if (!playerManager) return;

        playerManager.playerHeath = -1000;
    }
}
