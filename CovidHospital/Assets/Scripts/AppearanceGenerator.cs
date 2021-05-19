using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public enum BodyPart { Hair, Head, Body }
public enum Direction { Front, Back, Right, Left }

public class AppearanceGenerator : MonoBehaviour
{
    public PawnData GeneratePawnData()
    {
        PawnData data = new PawnData();
        data.HeadId = SpriteManager.GetRandomBodyPartId(BodyPart.Head);
        data.HairId = SpriteManager.GetRandomBodyPartId(BodyPart.Hair);
        data.BodyId = SpriteManager.GetRandomBodyPartId(BodyPart.Body);

        return data;
    }

    static private GameObject GenerateBodyPart(Sprite sprite, BodyPart bodyPart, GameObject parent) // zwraca dict 
    {
        GameObject obj = new GameObject();
        obj.name = bodyPart.ToString();
        obj.transform.parent = parent.transform;

        obj.AddComponent<SpriteRenderer>();
        var spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        if (bodyPart == BodyPart.Hair)
            spriteRenderer.sortingOrder = 1;
        return obj;
    }

    public GameObject GeneratePawnGameObject(PawnData data)
    {
        GameObject parent = new GameObject();
        parent.name = "Pawn";
        SpriteRenderer spriteRenderer = new SpriteRenderer();

        Pawn pawn = new Pawn(data);

        GenerateBodyPart(pawn.Hair, BodyPart.Hair, parent);
        GenerateBodyPart(pawn.Head, BodyPart.Head, parent);
        GenerateBodyPart(pawn.Body, BodyPart.Body, parent);

        return parent;
    }

    private void Turn(GameObject pawn, PawnData data, Direction direction)
    {
        pawn.transform.Find(BodyPart.Hair.ToString()).GetComponent<SpriteRenderer>().sprite = SpriteManager.GetPawnSprite(data.HairId, BodyPart.Hair, direction);
        pawn.transform.Find(BodyPart.Head.ToString()).GetComponent<SpriteRenderer>().sprite = SpriteManager.GetPawnSprite(data.HeadId, BodyPart.Head, direction);
        pawn.transform.Find(BodyPart.Body.ToString()).GetComponent<SpriteRenderer>().sprite = SpriteManager.GetPawnSprite(data.BodyId, BodyPart.Body, direction);
    }

    // Update is called once per frame
    private bool _loaded = false;
    void Update()
    {
        if (SpriteManager.AllSpritesLoaded && _loaded == false)
        {
            _loaded = true;
            var data = GeneratePawnData();
            var pawn = GeneratePawnGameObject(data);
        }
    }
}
