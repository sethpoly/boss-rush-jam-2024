using UnityEngine;

public class TouchMovementController: MonoBehaviour 
{
    // For touch support
    private Vector3 screenPoint;
    private Vector3 offset;

    public bool isDragging = false;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        Debug.Log("On Mouse Down");
        isDragging = true;
    }
 
    void OnMouseDrag()
    {
        Vector3 cursorScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint (cursorScreenPoint) + offset;
        transform.position = cursorPosition;
        Debug.Log("Mouse Drag to " + cursorPosition);
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}