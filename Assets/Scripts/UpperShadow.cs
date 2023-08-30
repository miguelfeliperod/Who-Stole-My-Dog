using UnityEngine;

public class UpperShadow : MonoBehaviour
{
    [SerializeField] SpriteRenderer originalSprite;
    SpriteRenderer upperShadowSprite;

    void Start()
    {
        upperShadowSprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        upperShadowSprite.sprite = originalSprite.sprite;
        upperShadowSprite.flipX = originalSprite.flipX;
        upperShadowSprite.transform.position= originalSprite.transform.position;
    }
}
