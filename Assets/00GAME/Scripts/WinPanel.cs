using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public Button nextButton;
    void Start()
    {
        nextButton.onClick.AddListener(NextLevel);
    }
    
    void NextLevel()
    {
        Debug.Log("Go To Next Level");
    }
}
