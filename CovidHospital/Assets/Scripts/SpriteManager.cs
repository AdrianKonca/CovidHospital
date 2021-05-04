using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class SpriteManager : MonoBehaviour
{
    static public bool AllSpritesLoaded { get; private set; }

    static public Dictionary<string, Sprite> WallSprites = new Dictionary<string, Sprite>();
    static public Dictionary<string, Sprite> TerrainSprites = new Dictionary<string, Sprite>();
    static private bool terrainTilesLoaded = false;
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
        terrainTilesLoaded = true;
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

        //string[] terrainNames = { "Grass", "Dirt" };
        //foreach (var terrainName in terrainNames)
        //{
        //    string terrainAddress = string.Format("Assets/Sprites/Terrain/Terrains.png[{0}]", terrainName);
        //    AsyncOperationHandle<Sprite> SpriteHandle = Addressables.LoadAssetAsync<Sprite>(terrainAddress);
        //    SpriteHandle.Completed += SpriteTerrain_Completed;
        //}

        var atlasAddress = "Assets/Sprites/Terrain/Terrain.spriteatlas";
        AsyncOperationHandle<SpriteAtlas> SpriteAtlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress);
        SpriteAtlasHandle.Completed += TerrainSpriteAtlas_Completed;

        _maxWallTiles = directions.Length * wallNames.Length;

    }

    // Update is called once per frame
    void Update()
    {
        if (AllSpritesLoaded)
            return;
        if (terrainTilesLoaded && _maxWallTiles == WallSprites.Count)
            AllSpritesLoaded = true;
    }
}
