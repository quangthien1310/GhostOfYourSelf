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

    public void OnRestartButtonClicked()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        
        Time.timeScale = 1;
    }

    public void OnLevel2ButtonClicked()
    {
        SceneManager.LoadScene("Level_2");
    }
    
    public void OnMenuButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameMenu");
    }
}
