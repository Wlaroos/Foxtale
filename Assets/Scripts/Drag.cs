using UnityEngine;

public class Drag : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            rb.MovePosition(mousePosition);
        }
    }
}
