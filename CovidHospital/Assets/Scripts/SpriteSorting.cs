using UnityEngine;

public class SpriteSorting : MonoBehaviour
{
    private const int yMult = 10;
    private const int yOffset = 100;
    private BodyPart BodyPart;
    private SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        if (name == "Body")
            BodyPart = BodyPart.Body;
        else if (name == "Head")
            BodyPart = BodyPart.Head;
        else if (name == "Hair")
            BodyPart = BodyPart.Hair;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (BodyPart == BodyPart.Hair)
            SpriteRenderer.sortingOrder = 29000 - Mathf.RoundToInt((transform.position.y + yOffset) * yMult) + 2;
        else
            SpriteRenderer.sortingOrder = 29000 - Mathf.RoundToInt((transform.position.y + yOffset) * yMult) + 1;
    }
}