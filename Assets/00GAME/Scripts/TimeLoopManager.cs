using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLoopManager : MonoBehaviour
{
    public static TimeLoopManager Instance;

    [Header("Settings")]
    public float maxRecordTime = 10f;
    public Transform startPoint;
    public bool oneTimeUse = true;
    public bool autoRewindOnTimeout = true;

    [Header("References")]
    public SpineRewind playerRecorder;
    public GhostRewinder ghostReplayer;
    
    [Header("UI")]
    [Tooltip("Kéo Image UI (Type = Filled) vào đây")]
    public Image timeBarImage; 

    private float currentRecordTime;
    private bool isRecording = false;
    private bool hasUsedRewind = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartRecording();
    }

    private void Update()
    {
        if (isRecording)
        {
            currentRecordTime += Time.deltaTime;
            
            if (timeBarImage != null)
            {
                float fillValue = 1f - (currentRecordTime / maxRecordTime);
                timeBarImage.fillAmount = Mathf.Clamp01(fillValue);
                
                if (fillValue < 0.3f) timeBarImage.color = Color.red;
                else timeBarImage.color = Color.white;
            }

            if (autoRewindOnTimeout && currentRecordTime >= maxRecordTime)
            {
                if (!hasUsedRewind)
                {
                    Debug.Log("Timeout! Auto Rewinding...");
                    PerformRewind();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (oneTimeUse && hasUsedRewind)
            {
                Debug.Log("Rewind already used!");
                return;
            }

            PerformRewind();
        }
    }

    private void StartRecording()
    {
        currentRecordTime = 0;
        isRecording = true;
        
        if (timeBarImage != null)
        {
            timeBarImage.fillAmount = 1f;
            timeBarImage.color = Color.white;
        }

        if (playerRecorder != null)
        {
            playerRecorder.StartRecording();
        }
    }

    private void PerformRewind()
    {
        Debug.Log("PERFORMING REWIND...");
        hasUsedRewind = true;
        isRecording = false;

        List<SpineRewind.FrameData> data = null;
        if (playerRecorder != null)
        {
            playerRecorder.StopRecording();
            data = playerRecorder.GetRecordedData();
        }

        if (playerRecorder != null && startPoint != null)
        {
            playerRecorder.transform.position = startPoint.position;
            var rb = playerRecorder.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (ghostReplayer != null && data != null && data.Count > 0)
        {
            ghostReplayer.StartReplay(data);
        }
        
        if (timeBarImage != null)
        {
            timeBarImage.gameObject.SetActive(false);
        }
    }
}
