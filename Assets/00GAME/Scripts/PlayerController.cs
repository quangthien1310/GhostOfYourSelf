using System;
using UnityEngine;
using Spine.Unity;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Check Ground")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 groundBoxSize = new Vector2(0.5f, 0.1f); // Kích thước vùng kiểm tra (Rộng x Cao)
    public float groundCastDistance = 0.05f; // Khoảng cách quét xuống dưới

    [Header("Spine")]
    public SkeletonAnimation skeleton;

    public GameObject winPanel;
    
    Rigidbody2D rb;

    bool isGrounded;
    bool isFacingRight = true;

    bool isInteracting;
    PushPullObject currentObject;

    float horizontal;
    bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (winPanel != null) winPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            jumpPressed = true;
        }

        CheckGround();
        HandleInteraction();
        HandleAnimation();
        HandleFlip();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // ================= MOVEMENT =================
    void HandleMovement()
    {
        if (isInteracting)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed * 0.5f, rb.linearVelocity.y);

            if (currentObject)
            {
                currentObject.rb.linearVelocity =
                    new Vector2(rb.linearVelocity.x, currentObject.rb.linearVelocity.y);
            }
            return;
        }

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    // ================= INTERACTION =================
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isInteracting)
            {
                StopInteraction();
            }
            else
            {
                TryInteract();
            }
        }
    }

    void TryInteract()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            isFacingRight ? Vector2.right : Vector2.left, 1f);

        if (hit && hit.collider.CompareTag("Pushable"))
        {
            if (!hit.collider.TryGetComponent<PushPullObject>(out currentObject)) return;

            isInteracting = true;
            rb.gravityScale = 0;
        }
    }

    void StopInteraction()
    {
        isInteracting = false;
        rb.gravityScale = 1;
        currentObject = null;
    }

    // ================= GROUND =================
    void CheckGround()
    {
        // Sử dụng BoxCast thay vì OverlapCircle để kiểm tra mặt đất tốt hơn trên dốc
        // Quét một hình hộp từ vị trí groundCheck xuống dưới một khoảng nhỏ
        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position, 
            groundBoxSize, 
            0f, 
            Vector2.down, 
            groundCastDistance, 
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    // ================= ANIMATION =================
    void HandleAnimation()
    {
        if (isInteracting)
        {
            if (horizontal != 0)
            {
                if ((horizontal > 0 && isFacingRight) ||
                    (horizontal < 0 && !isFacingRight))
                {
                    PlayAnim("push");
                }
                else
                {
                    PlayAnim("keo");
                }
            }
            else
            {
                PlayAnim(null);
            }
            return;
        }

        if (!isGrounded)
        {
            PlayAnim("jump");
            return;
        }

        if (horizontal != 0)
            PlayAnim("pull");
        else
            PlayAnim(null);
    }

    void PlayAnim(string anim)
    {
        if (skeleton == null) return;

        if (string.IsNullOrEmpty(anim))
        {
            if (skeleton.AnimationState.GetCurrent(0) != null)
            {
                skeleton.AnimationState.SetEmptyAnimation(0, 0.1f);
            }
            return;
        }

        if (skeleton.AnimationName == anim) return;
        skeleton.AnimationState.SetAnimation(0, anim, true);
    }

    // ================= FLIP =================
    void HandleFlip()
    {
        if (horizontal > 0 && !isFacingRight)
            Flip();
        else if (horizontal < 0 && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("End"))
        {
            winPanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            // Vẽ vùng BoxCast để dễ debug
            Gizmos.DrawWireCube(
                groundCheck.position + Vector3.down * groundCastDistance * 0.5f, 
                groundBoxSize
            );
        }
    }
}
