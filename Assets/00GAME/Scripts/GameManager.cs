using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")] 
    public Transform playerTransform;
    public Rigidbody2D playerRb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    
    public void OnMenuButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameMenu");
    }

    public void OnLevel2ButtonClicked()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Level_2");
    }
}
