using UnityEngine;

public class PushPullObject : MonoBehaviour
{
    public bool canBePulled = true;
    public bool canBePushed = true;

    [HideInInspector] public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }
}