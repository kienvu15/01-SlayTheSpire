using UnityEngine;

public class PanCamera : MonoBehaviour
{
    public float dragSpeed = 2f;   // tốc độ kéo
    public float scrollSpeed = 5f; // tốc độ di chuyển khi lăn chuột
    public Vector2 minBounds;      // giới hạn min X,Y
    public Vector2 maxBounds;      // giới hạn max X,Y

    private Vector3 dragOrigin;
    private Transform BossPanel;
    void Update()
    {
        HandleDrag();
        HandleScroll();
        
    }

    private void HandleDrag()
    {
        // Khi nhấn chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

            transform.Translate(move, Space.World);

            ClampPosition();

            dragOrigin = Input.mousePosition;
        }
    }

    private void HandleScroll()
    {
        float scroll = Input.mouseScrollDelta.y; // lăn chuột (thường là +1 hoặc -1)
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Mặc định cho nó di chuyển theo trục Y
            Vector3 move = new Vector3(0, scroll * scrollSpeed, 0);

            transform.Translate(move, Space.World);

            ClampPosition();
        }
    }

    private void ClampPosition()
    {
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minBounds.x, maxBounds.x);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minBounds.y, maxBounds.y);
        transform.position = clampedPos;
    }
}
