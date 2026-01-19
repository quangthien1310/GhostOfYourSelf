using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineRewind : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    
    // Dữ liệu ghi hình cho vòng lặp
    public struct FrameData
    {
        public float time;
        public Vector3 position;
        public Vector3 scale;
        public string animationName;
        public bool loop;
        public float trackTime;
    }

    private List<FrameData> recordedFrames = new List<FrameData>();
    private bool isRecording = false;
    private float startTime;

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    public void StartRecording()
    {
        recordedFrames.Clear();
        isRecording = true;
        startTime = Time.time;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public List<FrameData> GetRecordedData()
    {
        return new List<FrameData>(recordedFrames);
    }

    private void FixedUpdate()
    {
        if (!isRecording || skeletonAnimation == null) return;

        var currentTrack = skeletonAnimation.AnimationState.GetCurrent(0);
        string animName = "";
        bool loop = false;
        float trackTime = 0;

        if (currentTrack != null && currentTrack.Animation != null)
        {
            animName = currentTrack.Animation.Name;
            if (animName == "<empty>") animName = "";
            loop = currentTrack.Loop;
            trackTime = currentTrack.TrackTime;
        }

        recordedFrames.Add(new FrameData
        {
            time = Time.time - startTime, // Lưu thời gian tương đối tính từ lúc bắt đầu loop
            position = transform.position,
            scale = transform.localScale,
            animationName = animName,
            loop = loop,
            trackTime = trackTime
        });
    }
}
