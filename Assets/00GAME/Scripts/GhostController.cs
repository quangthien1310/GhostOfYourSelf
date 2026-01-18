using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class GhostController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Settings")]
    public float delay = 3.0f;

    [Header("Ghost Components")]
    public SkeletonAnimation ghostSkeleton;

    private PlayerController2D targetController;
    private Queue<GhostState> stateQueue = new Queue<GhostState>();

    private struct GhostState
    {
        public Vector3 position;
        public Vector3 scale;
        public string animationName;
        public float time;
    }

    private void Start()
    {
        if (target == null && GameManager.Instance != null && GameManager.Instance.playerTransform != null)
        {
            target = GameManager.Instance.playerTransform;
        }

        if (target != null)
        {
            targetController = target.GetComponent<PlayerController2D>();
        }

        if (ghostSkeleton == null)
        {
            ghostSkeleton = GetComponentInChildren<SkeletonAnimation>();
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
        
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (ghostSkeleton != null)
        {
            ghostSkeleton.skeleton.A = 0.5f;
        }
    }

    void Update()
    {
        if (target == null) return;

        // Record state
        string currentAnim = "";
        if (targetController != null && targetController.skeleton != null)
        {
            currentAnim = targetController.skeleton.AnimationName;
        }
        
        stateQueue.Enqueue(new GhostState
        {
            position = target.position,
            scale = target.localScale,
            animationName = currentAnim,
            time = Time.time
        });

        while (stateQueue.Count > 0)
        {
            GhostState state = stateQueue.Peek();
            if (Time.time - state.time >= delay)
            {
                stateQueue.Dequeue();
                ApplyState(state);
            }
            else
            {
                break;
            }
        }
    }

    void ApplyState(GhostState state)
    {
        transform.position = state.position;
        transform.localScale = state.scale;

        if (ghostSkeleton != null && !string.IsNullOrEmpty(state.animationName))
        {
            if (ghostSkeleton.AnimationName != state.animationName)
            {
                ghostSkeleton.AnimationState.SetAnimation(0, state.animationName, true);
            }
        }
    }
}
