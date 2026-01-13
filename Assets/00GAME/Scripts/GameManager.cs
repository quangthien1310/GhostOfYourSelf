using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")] 
    public Transform playerTransform;
    public Rigidbody2D playerRb;
    public Transform startPoint;
    public Collider endPoint;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Respawn();
    }

    public void Respawn()
    {
        if (startPoint == null || playerTransform == null) return;

        playerTransform.position = startPoint.position;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player Spawned!");
    }
}