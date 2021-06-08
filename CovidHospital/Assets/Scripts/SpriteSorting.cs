using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorting : MonoBehaviour
{
    private BodyPart BodyPart;
    private SpriteRenderer SpriteRenderer;
    const int yMult = 10;
    const int yOffset = 100;
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
        {
            SpriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y + yOffset) * yMult) + 2;
        }
        else
        {
            SpriteRenderer.sortingOrder = Mathf.RoundToInt((transform.position.y + yOffset) * yMult) + 1;
        }
    }

}
