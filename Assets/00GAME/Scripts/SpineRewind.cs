using UnityEngine;
using Spine.Unity;

public class SpineRewind : RewindAbstract
{
    private SkeletonAnimation skeletonAnimation;
    private CircularBuffer<SpineAnimationState> trackedSpineStates;

    public struct SpineAnimationState
    {
        public string animationName;
        public float trackTime;
        public bool loop;
    }

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        trackedSpineStates = new CircularBuffer<SpineAnimationState>();
    }

    public override void Track()
    {
        TrackTransform();
        TrackSpine();
    }

    public override void Rewind(float seconds)
    {
        RestoreTransform(seconds);
        RestoreSpine(seconds);
    }

    private void TrackSpine()
    {
        if (skeletonAnimation == null) return;

        var currentTrack = skeletonAnimation.AnimationState.GetCurrent(0);
        SpineAnimationState state = new SpineAnimationState();

        if (currentTrack != null && currentTrack.Animation != null)
        {
            string animName = currentTrack.Animation.Name;
            // Spine uses "<empty>" for empty animations, treat as empty string
            if (animName == "<empty>") animName = "";
            
            state.animationName = animName;
            state.trackTime = currentTrack.TrackTime;
            state.loop = currentTrack.Loop;
        }
        else
        {
            state.animationName = "";
            state.trackTime = 0;
            state.loop = false;
        }

        trackedSpineStates.WriteLastValue(state);
    }

    private void RestoreSpine(float seconds)
    {
        if (skeletonAnimation == null) return;

        SpineAnimationState state = trackedSpineStates.ReadFromBuffer(seconds);

        // Handle empty/null as "Stop animation" / "Setup pose"
        if (string.IsNullOrEmpty(state.animationName) || state.animationName == "<empty>")
        {
            var current = skeletonAnimation.AnimationState.GetCurrent(0);
            if (current != null && current.Animation != null && current.Animation.Name != "<empty>")
            {
                skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0.1f);
            }
            return;
        }

        var currentTrack = skeletonAnimation.AnimationState.GetCurrent(0);
        
        // If animation changed, set new animation
        if (currentTrack == null || currentTrack.Animation == null || currentTrack.Animation.Name != state.animationName)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, state.animationName, state.loop);
            currentTrack = skeletonAnimation.AnimationState.GetCurrent(0);
        }

        // Sync time
        if (currentTrack != null)
        {
            currentTrack.TrackTime = state.trackTime;
            skeletonAnimation.Update(0); // Force update to apply changes immediately
        }
    }

    public SpineAnimationState GetSpineSnapshot(float seconds)
    {
        return trackedSpineStates.ReadFromBuffer(seconds);
    }
}
