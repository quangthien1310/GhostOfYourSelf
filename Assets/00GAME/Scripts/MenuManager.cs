using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tên của Scene gameplay cần chuyển tới")]
    public string gameplaySceneName = "Level_1";

    public void OnStartButtonClicked()
    {
        if (Application.CanStreamedLevelBeLoaded(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            Debug.LogError($"Scene '{gameplaySceneName}' không tìm thấy! Hãy kiểm tra tên và thêm vào Build Settings.");
        }
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
