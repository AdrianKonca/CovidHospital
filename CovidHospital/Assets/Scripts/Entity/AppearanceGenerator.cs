using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;


public class PawnData : ScriptableObject 
{
    public int HairId;
    public int HeadId;
    public int BodyId;

}

public enum BodyPart { Hair, Head, Body}
public enum Direction { Front, Back, Right, Left }


//actual object in the game
public class Pawn : MonoBehaviour
{
    //Dictionary<Direction, Sprite> UpperBodySprites;
    public Sprite Hair;
    public Sprite Head;
    public Sprite Body;
    
    public Pawn(PawnData data)
    {
        Hair = AppearanceGenerator.spritesGlobal[(data.HairId, BodyPart.Hair, Direction.Front)];
        Head = AppearanceGenerator.spritesGlobal[(data.HeadId, BodyPart.Head, Direction.Front)];
        Body = AppearanceGenerator.spritesGlobal[(data.BodyId, BodyPart.Body, Direction.Front)];

    }

    
}

public class AppearanceGenerator : MonoBehaviour
{
    public static Dictionary<(int, BodyPart, Direction), Sprite> spritesGlobal = new Dictionary<(int, BodyPart, Direction), Sprite>();
    public static Dictionary<BodyPart, HashSet<int>> BodyPartId = new Dictionary<BodyPart, HashSet<int>>
    {
        {  BodyPart.Hair, new HashSet<int>() },
        {  BodyPart.Head, new HashSet<int>() },
        {  BodyPart.Body, new HashSet<int>() },

    };



    public int GetRandomBodyPartId(BodyPart bodyPart)
    {
        var randomBodyPartId = UnityEngine.Random.Range(0, BodyPartId[bodyPart].Count);
        return BodyPartId[bodyPart].ElementAt(randomBodyPartId);
    }
    public PawnData GeneratePawnData(){
        
        PawnData data = new PawnData();
        data.HeadId = GetRandomBodyPartId(BodyPart.Head);
        data.HairId = GetRandomBodyPartId(BodyPart.Hair);
        data.BodyId = GetRandomBodyPartId(BodyPart.Body);


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


    public GameObject GeneratePawnGameObject(PawnData data){
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
        pawn.transform.Find(BodyPart.Hair.ToString()).GetComponent<SpriteRenderer>().sprite = spritesGlobal[(data.HairId, BodyPart.Hair, direction)];
        pawn.transform.Find(BodyPart.Head.ToString()).GetComponent<SpriteRenderer>().sprite = spritesGlobal[(data.HeadId, BodyPart.Head, direction)];
        pawn.transform.Find(BodyPart.Body.ToString()).GetComponent<SpriteRenderer>().sprite = spritesGlobal[(data.BodyId, BodyPart.Body, direction)];



    }

    // Update is called once per frame
    private bool _loaded = false;
    void Update()
    {
        if(SpriteManager.AllSpritesLoaded && _loaded == false)
        {
            _loaded = true;
            var data = GeneratePawnData();
            var pawn = GeneratePawnGameObject(data);
        }
    }
}
