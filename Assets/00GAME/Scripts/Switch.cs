using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo GameObject Cánh Cửa (Door) cần mở vào đây")]
    public Transform doorObject; 
    
    [Tooltip("Kéo GameObject con (cái nút bấm) vào đây")]
    public Transform switchButton;
    
    [Header("Settings")]
    public float pressDepth = 0.15f;
    public float buttonSpeed = 5f;
    public float doorSpeed = 3f;

    private int objectsOnSwitch = 0;
    
    private Vector3 btnInitialPos;
    private Vector3 btnTargetPos;

    private Vector3 doorClosedPos;
    private Vector3 doorOpenPos;
    private Vector3 doorTargetPos;

    void Start()
    {
        if (switchButton != null)
        {
            btnInitialPos = switchButton.localPosition;
            btnTargetPos = btnInitialPos;
        }

        if (doorObject != null)
        {
            doorClosedPos = doorObject.localPosition;
            
            float height = 3.4f;
            
            // var col = doorObject.GetComponent<Collider2D>();
            // if (col != null) height = col.bounds.size.y;
            // else
            // {
            //     var spr = doorObject.GetComponent<SpriteRenderer>();
            //     if (spr != null) height = spr.bounds.size.y;
            // }

            doorOpenPos = doorClosedPos + new Vector3(0, height, 0);
            
            doorTargetPos = doorClosedPos;
        }
    }

    void Update()
    {
        if (switchButton != null)
        {
            switchButton.localPosition = Vector3.MoveTowards(
                switchButton.localPosition, 
                btnTargetPos, 
                buttonSpeed * Time.deltaTime
            );
        }

        if (doorObject != null)
        {
            doorObject.localPosition = Vector3.MoveTowards(
                doorObject.localPosition, 
                doorTargetPos, 
                doorSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidObject(other.gameObject))
        {
            AddObject();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidObject(other.gameObject))
        {
            RemoveObject();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsValidObject(collision.gameObject))
        {
            AddObject();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsValidObject(collision.gameObject))
        {
            RemoveObject();
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

    private void AddObject()
    {
        objectsOnSwitch++;
        if (objectsOnSwitch == 1)
        {
            SetState(true);
        }
    }

    private void RemoveObject()
    {
        objectsOnSwitch--;
        if (objectsOnSwitch < 0) objectsOnSwitch = 0;

        if (objectsOnSwitch == 0)
        {
            SetState(false);
        }
    }

    private void SetState(bool isActive)
    {
        if (switchButton != null)
        {
            if (isActive)
                btnTargetPos = new Vector3(btnInitialPos.x, btnInitialPos.y - pressDepth, btnInitialPos.z);
            else
                btnTargetPos = btnInitialPos;
        }

        if (doorObject != null)
        {
            if (isActive)
                doorTargetPos = doorOpenPos;
            else
                doorTargetPos = doorClosedPos;
        }
    }
}
