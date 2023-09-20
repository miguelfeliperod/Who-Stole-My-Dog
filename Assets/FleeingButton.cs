using UnityEngine;

public class FleeingButton : MonoBehaviour
{
    RectTransform rectTransform;
    float initialX;
    float initialY;

    float xNewPosition;
    float yNewPosition;

    float xOffset;
    float yOffset;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        initialX = rectTransform.position.x;
        initialY = rectTransform.position.y;
    }

    public void JumpFarFromCursor()
    {
        xOffset = (Random.Range(2 * rectTransform.sizeDelta.x, 3 * rectTransform.sizeDelta.x)) * (Random.Range(0, 2) == 0 ? 1 : -1);
        yOffset = (Random.Range(4 * rectTransform.sizeDelta.y, 6 * rectTransform.sizeDelta.y)) * (Random.Range(0, 2) == 0 ? 1 : -1);

        xNewPosition = rectTransform.position.x + xOffset;
        yNewPosition = rectTransform.position.y + yOffset;

        if (xNewPosition > initialX + rectTransform.sizeDelta.x * 6 ||
            xNewPosition < initialX - rectTransform.sizeDelta.x * 6)
            xNewPosition = initialX;

        if (yNewPosition > initialY + rectTransform.sizeDelta.y * 8 ||
            yNewPosition < initialY - rectTransform.sizeDelta.y * 8)
            yNewPosition = initialY;

        rectTransform.position = new Vector3(xNewPosition, yNewPosition);
    }
}
