using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class GhostRewinder : MonoBehaviour
{
    [Header("Ghost Components")]
    public SkeletonAnimation ghostSkeleton;

    private List<SpineRewind.FrameData> replayData;
    private bool isReplaying = false;
    private float replayStartTime;
    private int currentIndex = 0;
    private Rigidbody2D rb;

    private void Start()
    {
        if (ghostSkeleton == null)
        {
            ghostSkeleton = GetComponent<SkeletonAnimation>();
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // --- CẤU HÌNH VẬT LÝ CHO GHOST ---
        // Kinematic: Không bị trọng lực, nhưng có thể đẩy các vật thể Dynamic khác (như Player)
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.useFullKinematicContacts = true; // Quan trọng: Để va chạm với Dynamic Body
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false; // Tắt Trigger để có va chạm vật lý thật (chặn đường)
        }

        if (ghostSkeleton != null)
        {
            ghostSkeleton.skeleton.A = 0.5f;
        }
        
        gameObject.SetActive(false);
    }

    public void StartReplay(List<SpineRewind.FrameData> data)
    {
        if (data == null || data.Count == 0) return;

        replayData = data;
        isReplaying = true;
        replayStartTime = Time.time;
        currentIndex = 0;
        
        gameObject.SetActive(true);
        
        // Set vị trí đầu tiên ngay lập tức
        transform.position = data[0].position;
        ApplyFrame(data[0]);
    }

    public void StopReplay()
    {
        isReplaying = false;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!isReplaying || replayData == null || replayData.Count == 0) return;

        float currentTime = Time.time - replayStartTime;

        while (currentIndex < replayData.Count - 1 && replayData[currentIndex + 1].time <= currentTime)
        {
            currentIndex++;
        }

        // Thay vì gán transform.position, ta dùng MovePosition để tương tác vật lý
        if (rb != null)
        {
            // Di chuyển vật lý đến vị trí của frame hiện tại
            rb.MovePosition(replayData[currentIndex].position);
        }
        else
        {
            transform.position = replayData[currentIndex].position;
        }

        ApplyFrame(replayData[currentIndex]);

        if (currentTime > replayData[replayData.Count - 1].time)
        {
            // Hết dữ liệu
        }
    }

    private void ApplyFrame(SpineRewind.FrameData frame)
    {
        // Scale vẫn gán trực tiếp vì Rigidbody không quản lý scale
        transform.localScale = frame.scale;

        if (ghostSkeleton == null) return;

        if (string.IsNullOrEmpty(frame.animationName))
        {
            var current = ghostSkeleton.AnimationState.GetCurrent(0);
            if (current != null && current.Animation != null && current.Animation.Name != "<empty>")
            {
                ghostSkeleton.AnimationState.SetEmptyAnimation(0, 0.1f);
            }
            return;
        }

        var currentTrack = ghostSkeleton.AnimationState.GetCurrent(0);
        
        if (currentTrack == null || currentTrack.Animation == null || currentTrack.Animation.Name != frame.animationName)
        {
            ghostSkeleton.AnimationState.SetAnimation(0, frame.animationName, frame.loop);
            currentTrack = ghostSkeleton.AnimationState.GetCurrent(0);
        }

        if (currentTrack != null)
        {
            currentTrack.TrackTime = frame.trackTime;
            ghostSkeleton.Update(0);
        }
    }
}
