using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;


public class SpriteManager : MonoBehaviour
{
    #region PawnsSprites
    static public Dictionary<(int, BodyPart, Direction), Sprite> PawnBodyPartSprites = new Dictionary<(int, BodyPart, Direction), Sprite>();
    static public Dictionary<BodyPart, HashSet<int>> BodyPartToId = new Dictionary<BodyPart, HashSet<int>>
    {
        {  BodyPart.Hair, new HashSet<int>() },
        {  BodyPart.Head, new HashSet<int>() },
        {  BodyPart.Body, new HashSet<int>() },

    };
    static private int _pawnAtlasesToLoad = 3;

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

            BodyPartToId[bodyPart].Add(bodyPartId);
            PawnBodyPartSprites.Add((
                bodyPartId,
                bodyPart,
                _directionMap[fileParts[2]]
            ), sprite);
        }
        _pawnAtlasesToLoad--;
    }
    #endregion

    static public bool AllSpritesLoaded { get; private set; }
    public List<string> WallNames = new List<string>();
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
        foreach (var wallName in WallNames)
        {
            foreach (var direction in directions)
            {
                string address = string.Format("Assets/Sprites/Walls/{0}.png[{0}_{1}]", wallName, direction);
                AsyncOperationHandle<Sprite> SpriteHandle = Addressables.LoadAssetAsync<Sprite>(address);
                SpriteHandle.Completed += WallSprite_Completed;
            }
        }
        _maxWallTiles = directions.Length * WallNames.Count;

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
        if (_terrainTilesLoaded && _furnitureTilesLoaded && _maxWallTiles == WallSprites.Count && _pawnAtlasesToLoad == 0)
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

    static public Sprite GetPawnSprite(int index, BodyPart bodyPart, Direction direction)
    {
        var key = (index, bodyPart, direction);
        if (PawnBodyPartSprites.ContainsKey(key))
            return PawnBodyPartSprites[key];
        Debug.LogWarning(string.Format("Pawn sprite not found {0}-{1}-{2}: ", index, bodyPart.ToString(), direction.ToString()));
        return null;
    }
    static public int GetRandomBodyPartId(BodyPart bodyPart)
    {
        var randomBodyPartId = UnityEngine.Random.Range(0, BodyPartToId[bodyPart].Count);
        return BodyPartToId[bodyPart].ElementAt(randomBodyPartId);
    }

}
