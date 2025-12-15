using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIFixedPosition : MonoBehaviour
{
    public enum ScreenPosition
    {
        TopLeft,
        TopRight
    }

    [Header("Settings")]
    public ScreenPosition targetPosition;

    // Adjust these to push the UI slightly away from the exact edge
    public Vector2 offset = new Vector2(20f, -20f);

    private RectTransform rectTransform;

    void Awake()
    {
        LockPosition();
    }

    // We run this in Update temporarily to fight against any other scripts 
    // that might be trying to move it. You can remove Update() later.
    void Update()
    {
        LockPosition();
    }

    void LockPosition()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        if (targetPosition == ScreenPosition.TopLeft)
        {
            // Anchor to Top-Left
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1); // Pivot at top-left corner

            // Set fixed position (e.g., x=20, y=-20)
            rectTransform.anchoredPosition = new Vector2(Mathf.Abs(offset.x), -Mathf.Abs(offset.y));
        }
        else if (targetPosition == ScreenPosition.TopRight)
        {
            // Anchor to Top-Right
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1); // Pivot at top-right corner

            // Set fixed position (e.g., x=-20, y=-20)
            // Note: X must be negative to move left from the right edge
            rectTransform.anchoredPosition = new Vector2(-Mathf.Abs(offset.x), -Mathf.Abs(offset.y));
        }
    }
}