using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;



public class SpriteManager //wczytuje wszystkie sprites do dictionary spritesGlobal
{

    public string []spriteAtlasAddress = new string[] {
        "Assets/Sprites/Pawns/Face.spriteatlas",
        "Assets/Sprites/Pawns/Hair.spriteatlas",
        "Assets/Sprites/Pawns/Head.spriteatlas",
        "Assets/Sprites/Pawns/LowerBody.spriteatlas",
        "Assets/Sprites/Pawns/UpperBody.spriteatlas"
    };

    private Dictionary<string, BodyPart> _bodyPartMap = new Dictionary<string, BodyPart>() {
        {"upperBody", BodyPart.UpperBody },
        {"lowerBody", BodyPart.LowerBody },
        {"face", BodyPart.Face },
        {"head", BodyPart.Head },
        {"hair", BodyPart.Hair },
    };

    private Dictionary<string, Direction> _directiontMap = new Dictionary<string, Direction>() {
        {"front", Direction.Front },
       {"back", Direction.Back }

    };
    public void Start()
    {
        foreach (var adress in spriteAtlasAddress)
        {
            string atlasedSpriteAddress = adress;
            Addressables.LoadAssetAsync<SpriteAtlas>(atlasedSpriteAddress).Completed += SpriteAtlasLoaded;
        }
        
    }
    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj) //get spirites 
    {
        string[] fileParts;
        Sprite []tmpSprites = new Sprite[obj.Result.spriteCount];
        obj.Result.GetSprites(tmpSprites);
        foreach (var sprite in tmpSprites){
            sprite.name = sprite.name.Replace("(Clone)", "");
            fileParts = sprite.name.Split("_".ToCharArray());
            
            var bodyPartId = int.Parse(fileParts[0]);
            var bodyPart = _bodyPartMap[fileParts[1]];

            AppearanceGenerator.BodyPartId[bodyPart].Add(bodyPartId);
            AppearanceGenerator.spritesGlobal.Add((
                bodyPartId, 
                bodyPart, 
                _directiontMap[fileParts[2]]
            ), sprite);


        }
    }

}
public class PawnData : ScriptableObject // trzyma id pojedyńczych części ciała
{
    public int UpperBodyId;
    public int LowerBodyId;
    public int FaceId;
    public int HeadId;
    public int HairId;
}

public enum BodyPart { Head, Hair, Face, LowerBody, UpperBody }
public enum Direction { Front, Back }


//actual object in the game
public class Pawn : MonoBehaviour
{
    //Dictionary<Direction, Sprite> UpperBodySprites;
    public Sprite UpperBody;
    public Sprite LowerBody;
    public Sprite Face;
    public Sprite Head;
    public Sprite Hair;
    
    public Pawn(PawnData data)
    {

        UpperBody = AppearanceGenerator.spritesGlobal[(data.UpperBodyId, BodyPart.UpperBody, Direction.Front)];
        LowerBody = AppearanceGenerator.spritesGlobal[(data.LowerBodyId, BodyPart.LowerBody, Direction.Front)];
        Face = AppearanceGenerator.spritesGlobal[(data.FaceId, BodyPart.Face, Direction.Front)];
        Head = AppearanceGenerator.spritesGlobal[(data.HeadId, BodyPart.Head, Direction.Front)];
        Hair = AppearanceGenerator.spritesGlobal[(data.HairId, BodyPart.Hair, Direction.Front)];
    }

    
}

public class AppearanceGenerator : MonoBehaviour
{
    public static Dictionary<(int, BodyPart, Direction), Sprite> spritesGlobal = new Dictionary<(int, BodyPart, Direction), Sprite>();
    public static Dictionary<BodyPart, HashSet<int>> BodyPartId = new Dictionary<BodyPart, HashSet<int>>
    {
        {  BodyPart.UpperBody, new HashSet<int>() },
        {  BodyPart.LowerBody, new HashSet<int>() },
        {  BodyPart.Face, new HashSet<int>() },
        {  BodyPart.Head, new HashSet<int>() },
        {  BodyPart.Hair, new HashSet<int>() },
    };



    public int GetRandomBodyPartId(BodyPart bodyPart)
    {
        var randomBodyPartId = UnityEngine.Random.Range(0, BodyPartId[bodyPart].Count);
        return BodyPartId[bodyPart].ElementAt(randomBodyPartId);
    }
    public PawnData GeneratePawnData(){
        
        PawnData data = new PawnData();

        data.UpperBodyId = GetRandomBodyPartId(BodyPart.UpperBody);
        data.LowerBodyId = GetRandomBodyPartId(BodyPart.LowerBody);
        data.FaceId = GetRandomBodyPartId(BodyPart.Face);
        data.HeadId = GetRandomBodyPartId(BodyPart.Head);
        data.HairId = GetRandomBodyPartId(BodyPart.Hair);

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
        if (bodyPart == BodyPart.Face || bodyPart == BodyPart.Hair)
            spriteRenderer.sortingOrder = 1;
        return obj;
    }


    public Pawn GeneratePawnGameObject(PawnData data){
        GameObject parent = new GameObject();
        parent.name = "Pawn";
        SpriteRenderer spriteRenderer = new SpriteRenderer();

        Pawn pawn = new Pawn(data);


        GenerateBodyPart(pawn.LowerBody, BodyPart.LowerBody, parent);
        GenerateBodyPart(pawn.UpperBody, BodyPart.UpperBody, parent);
        GenerateBodyPart(pawn.Face, BodyPart.Face, parent);
        GenerateBodyPart(pawn.Head, BodyPart.Head, parent);
        GenerateBodyPart(pawn.Hair, BodyPart.Hair, parent);

        return pawn;
    }

    SpriteManager spriteManager = new SpriteManager();
    // Start is called before the first frame update
    void Start(){
        spriteManager.Start();
    }

    // Update is called once per frame

    private float _waitForLoading = 5f;
    private bool _loaded = false;
    void Update()
    {
        _waitForLoading -= Time.deltaTime;
        if (_waitForLoading < 0 && !_loaded)
        {
            _loaded = true;
            PawnData data = GeneratePawnData();
            Pawn pawn = GeneratePawnGameObject(data);
        }

    }
}
