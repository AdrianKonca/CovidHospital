using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;


public class SpriteManager : MonoBehaviour
{
    //Roksana's part

    public string[] spriteAtlasAddress = new string[] {
        "Assets/Sprites/Pawns/Hair.spriteatlas",
        "Assets/Sprites/Pawns/Head.spriteatlas",
        "Assets/Sprites/Pawns/Body.spriteatlas",
    };

    private Dictionary<string, BodyPart> _bodyPartMap = new Dictionary<string, BodyPart>() {
        {"hair", BodyPart.Hair },
        {"head", BodyPart.Head },
        {"body", BodyPart.Body },

    };

    private Dictionary<string, Direction> _directionMap = new Dictionary<string, Direction>() {
        {"front", Direction.Front },
        {"back", Direction.Back },
        {"right", Direction.Right },
        {"left", Direction.Left }
    };

    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj) //get spirites 
    {
        string[] fileParts;
        Sprite[] tmpSprites = new Sprite[obj.Result.spriteCount];
        obj.Result.GetSprites(tmpSprites);
        foreach (var sprite in tmpSprites)
        {
            sprite.name = sprite.name.Replace("(Clone)", "");
            fileParts = sprite.name.Split("_".ToCharArray());

            var bodyPartId = int.Parse(fileParts[0]);
            var bodyPart = _bodyPartMap[fileParts[1]];

            AppearanceGenerator.BodyPartId[bodyPart].Add(bodyPartId);
            AppearanceGenerator.spritesGlobal.Add((
                bodyPartId,
                bodyPart,
                _directionMap[fileParts[2]]
            ), sprite);
        }
        pawnAtlasesLoaded--;
    }
    //mixed
    static private int pawnAtlasesLoaded = 3;

    //Adrian's part
    static public bool AllSpritesLoaded { get; private set; }

    static public Dictionary<string, Sprite> WallSprites = new Dictionary<string, Sprite>();
    static public Dictionary<string, Sprite> TerrainSprites = new Dictionary<string, Sprite>();
    static public Dictionary<string, Sprite> FurnitureSprites = new Dictionary<string, Sprite>();
    static private bool _terrainTilesLoaded = false;
    static private bool _furnitureTilesLoaded = false;
    static private int _maxWallTiles = 0;

    private void TerrainSpriteAtlas_Completed(AsyncOperationHandle<SpriteAtlas> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            SpriteAtlas result = handle.Result;
            var sprites = new Sprite[result.spriteCount];
            result.GetSprites(sprites);
            foreach (var sprite in sprites)
            {
                //TODO: Investigate why sprites are initialized with (Clone) postfix;
                sprite.name = sprite.name.Replace("(Clone)", "");
                TerrainSprites[sprite.name] = sprite;
            }
        }
        Debug.Log("All terrain sprites loaded");
        _terrainTilesLoaded = true;
    }

    private void FurnitureSpriteAtlas_Completed(AsyncOperationHandle<SpriteAtlas> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            SpriteAtlas result = handle.Result;
            var sprites = new Sprite[result.spriteCount];
            result.GetSprites(sprites);
            foreach (var sprite in sprites)
            {
                //TODO: Investigate why sprites are initialized with (Clone) postfix;
                sprite.name = sprite.name.Replace("(Clone)", "");
                FurnitureSprites[sprite.name] = sprite;
            }
        }
        Debug.Log("All furniture sprites loaded");
        _furnitureTilesLoaded = true;
    }

    private void WallSprite_Completed(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite result = handle.Result;
            WallSprites[result.name] = result;
        }

    }
    void Start()
    {
        string[] directions = { "", "E", "ES", "ESW", "EW", "N", "NE", "NES", "NESW", "NEW", "NS", "NSW", "NW", "S", "SW", "W" };
        string[] wallNames = { "ConcreteWall" };
        foreach (var wallName in wallNames)
        {
            foreach (var direction in directions)
            {
                string address = string.Format("Assets/Sprites/Walls/{0}.png[{0}_{1}]", wallName, direction);
                AsyncOperationHandle<Sprite> SpriteHandle = Addressables.LoadAssetAsync<Sprite>(address);
                SpriteHandle.Completed += WallSprite_Completed;
            }
        }
        _maxWallTiles = directions.Length * wallNames.Length;

        var atlasAddress = "Assets/Sprites/Terrain/Terrain.spriteatlas";
        AsyncOperationHandle<SpriteAtlas> SpriteAtlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress);
        SpriteAtlasHandle.Completed += TerrainSpriteAtlas_Completed;

        atlasAddress = "Assets/Sprites/Objects/Furniture.spriteatlas";
        SpriteAtlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress);
        SpriteAtlasHandle.Completed += FurnitureSpriteAtlas_Completed;

        foreach (var adress in spriteAtlasAddress)
        {
            string atlasedSpriteAddress = adress;
            Addressables.LoadAssetAsync<SpriteAtlas>(atlasedSpriteAddress).Completed += SpriteAtlasLoaded;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AllSpritesLoaded)
            return;
        if (_terrainTilesLoaded && _furnitureTilesLoaded && _maxWallTiles == WallSprites.Count && pawnAtlasesLoaded == 0)
            AllSpritesLoaded = true;
    }

    static public Sprite GetWallSpriteByName(string name)
    {
        if (WallSprites.ContainsKey(name + "_"))
            return WallSprites[name + "_"];
        Debug.LogWarning("Wall sprite not found: " + name + "_");
        return null;
    }
    static public Sprite GetTerrainSpriteByName(string name)
    {
        if (TerrainSprites.ContainsKey(name))
            return TerrainSprites[name];
        Debug.LogWarning("Terrain sprite not found: " + name);
        return null;
    }

    static public Sprite GetFurnitureSpriteByName(string name)
    {
        if (FurnitureSprites.ContainsKey(name))
            return FurnitureSprites[name];
        Debug.LogWarning(string.Format("Furniture sprite not found: " + name));
        return null;
    }
}
