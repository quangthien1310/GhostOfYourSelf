using UnityEngine;
using Spine.Unity;

public class GhostRewinder : MonoBehaviour
{
    [Header("Target")]
    public SpineRewind targetRewind;
    
    [Header("Settings")]
    public float delay = 1.0f; // Delay in seconds

    [Header("Ghost Components")]
    public SkeletonAnimation ghostSkeleton;

    private void Start()
    {
        // Auto-assign target if not set
        if (targetRewind == null && GameManager.Instance != null && GameManager.Instance.playerTransform != null)
        {
            targetRewind = GameManager.Instance.playerTransform.GetComponent<SpineRewind>();
        }

        if (ghostSkeleton == null)
        {
            ghostSkeleton = GetComponent<SkeletonAnimation>();
        }

        // --- CẤU HÌNH VẬT LÝ CHO GHOST ---
        // Để Ghost có thể kích hoạt Switch (Trigger), nó cần Rigidbody2D và Collider2D
        
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // Không bị trọng lực rơi
            rb.simulated = true; // Phải bật simulated mới tương tác được Trigger
        }
        
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true; // Ghost là bóng ma, đi xuyên qua mọi thứ
        }

        // Set transparency
        if (ghostSkeleton != null)
        {
            ghostSkeleton.skeleton.A = 0.5f;
        }
    }

    private void FixedUpdate()
    {
        if (targetRewind == null) return;

        // Apply Transform
        var transformState = targetRewind.GetTransformSnapshot(delay);
        transform.position = transformState.position;
        transform.rotation = transformState.rotation;
        transform.localScale = transformState.scale;

        // Apply Animation
        if (ghostSkeleton != null)
        {
            var spineState = targetRewind.GetSpineSnapshot(delay);
            ApplySpineState(spineState);
        }
    }

    private void ApplySpineState(SpineRewind.SpineAnimationState state)
    {
        // Handle empty/null as "Stop animation" / "Setup pose"
        if (string.IsNullOrEmpty(state.animationName) || state.animationName == "<empty>")
        {
            var current = ghostSkeleton.AnimationState.GetCurrent(0);
            if (current != null && current.Animation != null && current.Animation.Name != "<empty>")
            {
                ghostSkeleton.AnimationState.SetEmptyAnimation(0, 0.1f);
            }
            return;
        }

        var currentTrack = ghostSkeleton.AnimationState.GetCurrent(0);
        
        // If animation changed, set new animation
        if (currentTrack == null || currentTrack.Animation == null || currentTrack.Animation.Name != state.animationName)
        {
            ghostSkeleton.AnimationState.SetAnimation(0, state.animationName, state.loop);
            currentTrack = ghostSkeleton.AnimationState.GetCurrent(0);
        }

        // Sync time
        if (currentTrack != null)
        {
            currentTrack.TrackTime = state.trackTime;
            ghostSkeleton.Update(0);
        }
    }
}
