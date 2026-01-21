using System.Collections.Generic;
using UnityEngine;

public class RotateSwitch : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Danh sách các mảnh cầu cần xoay")]
    public List<Transform> bridges;
    
    [Tooltip("Kéo GameObject con (cái nút bấm) vào đây")]
    public Transform switchButton;
    
    [Header("Settings")]
    public float pressDepth = 0.15f; // Độ sâu lún xuống
    public float buttonSpeed = 10f;  // Tốc độ nút nảy
    public float bridgeRotateSpeed = 180f; // Tốc độ xoay cầu (độ/giây)

    // Trạng thái nút
    private Vector3 btnInitialPos;
    private Vector3 btnPressedPos;
    private Vector3 btnTargetPos;
    private float pressTimer = 0f;

    // Trạng thái cầu
    private List<Quaternion> targetRotations = new List<Quaternion>();

    void Start()
    {
        // Setup Nút
        if (switchButton != null)
        {
            btnInitialPos = switchButton.localPosition;
            btnPressedPos = new Vector3(btnInitialPos.x, btnInitialPos.y - pressDepth, btnInitialPos.z);
            btnTargetPos = btnInitialPos;
        }

        // Setup Cầu
        foreach (var bridge in bridges)
        {
            if (bridge != null)
            {
                targetRotations.Add(bridge.rotation);
            }
            else
            {
                targetRotations.Add(Quaternion.identity); // Placeholder
            }
        }
    }

    void Update()
    {
        // 1. Hiệu ứng nút bấm (Tự nảy lên sau khi nhấn)
        if (switchButton != null)
        {
            if (pressTimer > 0)
            {
                pressTimer -= Time.deltaTime;
                if (pressTimer <= 0)
                {
                    btnTargetPos = btnInitialPos; // Nảy lên
                }
            }

            switchButton.localPosition = Vector3.MoveTowards(
                switchButton.localPosition, 
                btnTargetPos, 
                buttonSpeed * Time.deltaTime
            );
        }

        // 2. Xoay cầu mượt mà
        for (int i = 0; i < bridges.Count; i++)
        {
            if (bridges[i] != null)
            {
                bridges[i].rotation = Quaternion.RotateTowards(
                    bridges[i].rotation, 
                    targetRotations[i], 
                    bridgeRotateSpeed * Time.deltaTime
                );
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidObject(other.gameObject))
        {
            PressButton();
        }
    }

    // Hỗ trợ cả Collision nếu cần (cho Player nhảy lên)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsValidObject(collision.gameObject))
        {
            // Kiểm tra hướng đứng từ trên xuống
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    PressButton();
                    break;
                }
            }
        }
    }

    private void PressButton()
    {
        // Hiệu ứng nút lún xuống
        if (switchButton != null)
        {
            btnTargetPos = btnPressedPos;
            pressTimer = 0.2f; // Giữ lún trong 0.2s rồi nảy lên
        }

        // Xoay cầu thêm 180 độ
        RotateBridges();
    }

    private void RotateBridges()
    {
        for (int i = 0; i < targetRotations.Count; i++)
        {
            // Cộng thêm 180 độ vào trục Z (cho game 2D)
            // Nếu cầu xoay trục khác, hãy sửa Vector3.forward thành Vector3.up hoặc right
            targetRotations[i] *= Quaternion.Euler(0, 0, 180);
        }
    }

    private bool IsValidObject(GameObject obj)
    {
        if (obj.CompareTag("Ground") || obj.CompareTag("Untagged")) return false;

        bool isPlayer = obj.CompareTag("Player");
        bool isGhost = obj.GetComponent<GhostRewinder>() != null;
        
        if (!isGhost)
        {
            isGhost = obj.GetComponentInParent<GhostRewinder>() != null;
        }

        return isPlayer || isGhost;
    }
}
