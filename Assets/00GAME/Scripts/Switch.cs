using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Kéo GameObject ExitPoint (có tag 'End') vào đây")]
    public Collider2D exitPoint; 
    public SpriteRenderer switchRenderer;
    
    [Header("Visuals")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    private int objectsOnSwitch = 0;

    void Start()
    {
        if (exitPoint != null)
        {
            exitPoint.isTrigger = false;
        }
        UpdateVisuals(false);
    }

    // --- XỬ LÝ TRIGGER ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Switch] Trigger Enter: {other.gameObject.name} | Tag: {other.tag} | Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
        
        if (IsValidObject(other.gameObject))
        {
            Debug.Log("-> HỢP LỆ (Trigger)");
            AddObject();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidObject(other.gameObject))
        {
            Debug.Log($"[Switch] Trigger Exit: {other.gameObject.name}");
            RemoveObject();
        }
    }

    // --- XỬ LÝ COLLISION ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Switch] Collision Enter: {collision.gameObject.name} | Tag: {collision.gameObject.tag} | Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");

        if (IsValidObject(collision.gameObject))
        {
            Debug.Log("-> HỢP LỆ (Collision)");
            AddObject();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsValidObject(collision.gameObject))
        {
            Debug.Log($"[Switch] Collision Exit: {collision.gameObject.name}");
            RemoveObject();
        }
    }

    private bool IsValidObject(GameObject obj)
    {
        // Bỏ qua Ground để đỡ spam log, nhưng vẫn log ở trên để kiểm tra
        if (obj.CompareTag("Ground")) return false;

        bool isPlayer = obj.CompareTag("Player");
        bool isGhost = obj.GetComponent<GhostRewinder>() != null;
        
        if (!isGhost)
        {
            isGhost = obj.GetComponentInParent<GhostRewinder>() != null;
        }

        if (!isPlayer && !isGhost)
        {
            Debug.Log($"-> KHÔNG HỢP LỆ: Không phải Player (Tag={obj.tag}) và không có GhostRewinder");
        }

        return isPlayer || isGhost;
    }

    private void AddObject()
    {
        objectsOnSwitch++;
        Debug.Log($"Objects on switch: {objectsOnSwitch}");
        
        if (objectsOnSwitch == 1)
        {
            SetState(true);
        }
    }

    private void RemoveObject()
    {
        objectsOnSwitch--;
        if (objectsOnSwitch < 0) objectsOnSwitch = 0;
        Debug.Log($"Objects on switch: {objectsOnSwitch}");

        if (objectsOnSwitch == 0)
        {
            SetState(false);
        }
    }

    private void SetState(bool isActive)
    {
        Debug.Log($"Set Switch State: {isActive}");
        if (exitPoint != null)
        {
            exitPoint.isTrigger = isActive;
        }
        UpdateVisuals(isActive);
    }

    private void UpdateVisuals(bool isActive)
    {
        if (switchRenderer != null)
        {
            switchRenderer.color = isActive ? activeColor : inactiveColor;
        }
        
        if (exitPoint != null)
        {
            var doorRenderer = exitPoint.GetComponent<SpriteRenderer>();
            if (doorRenderer != null)
            {
                doorRenderer.color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}
