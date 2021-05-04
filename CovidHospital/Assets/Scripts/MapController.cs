﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class TileWall : Tile
{
    public string wallName { get; set; }
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        SelectNewSprite(position, tilemap, true);
    }
    private void SelectNewSprite(Vector3Int position, ITilemap tilemap, bool continueToNeighbors)
    {
        string spriteName = wallName + "_";
        string[] directions = { "N", "E", "S", "W" };
        Dictionary<string, Vector3Int> neighobrs = new Dictionary<string, Vector3Int>
        {
            { "N", new Vector3Int(position.x, position.y + 1, position.z) },
            { "E", new Vector3Int(position.x + 1, position.y, position.z) },
            { "S", new Vector3Int(position.x, position.y - 1, position.z) },
            { "W", new Vector3Int(position.x - 1, position.y, position.z) },
        };
        foreach (var direction in directions)
        {
            if (IsNeighbour(neighobrs[direction], tilemap))
            {
                spriteName += direction;
                if (continueToNeighbors)
                    SelectNewSprite(neighobrs[direction], tilemap, false);
            }
        }

        if (!continueToNeighbors)
        {
            TileWall me = tilemap.GetTile<TileWall>(position);
            me.sprite = MapController.WallSprites[spriteName];
        }
        else
        {
            sprite = MapController.WallSprites[spriteName];
        }
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.colliderType = ColliderType.Grid;
    }

    private bool IsNeighbour(Vector3Int position, ITilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(position);
        return (tile != null);
    }
}

public class MapController : MonoBehaviour
{
    public Tilemap Terrain;
    public Tilemap Walls;
    static public Dictionary<string, Sprite> WallSprites = new Dictionary<string, Sprite>();
    static public Dictionary<string, Sprite> TerrainSprites = new Dictionary<string, Sprite>();
    private int _maxWallTiles = 0;
    private int _maxTerrainTiles = 0;
    private string TERRAIN_AFTER_WALL_DECON = "Concrete";
    // Start is called before the first frame update

    private void SpriteAtlas_Completed(AsyncOperationHandle<SpriteAtlas> handle)
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
    }

    private void Sprite_Completed(AsyncOperationHandle<Sprite> handle)
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
                SpriteHandle.Completed += Sprite_Completed;
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

        SpriteAtlasHandle.Completed += SpriteAtlas_Completed;
        _maxTerrainTiles = 3;
        _maxWallTiles = directions.Length * wallNames.Length;

    }

    private bool _mapInitialized = false;

    string WALL_NAME = "ConcreteWall";

    private IEnumerator LoadTerrain()
    {
        int MAP_LIMIT = 100;

        var terrain_names = new List<string>{"Grass", "Dirt", "Concrete"};
        var terrain_tiles = new List<Tile>();
        foreach (var name in terrain_names)
        {
            var terrain = ScriptableObject.CreateInstance<Tile>();
            terrain.sprite = TerrainSprites[name];
            terrain_tiles.Add(terrain);
        }
        for (int x = -MAP_LIMIT; x < MAP_LIMIT; x++)
        {
            for (int y = -MAP_LIMIT; y < MAP_LIMIT; y++)
            {
                float noise = Mathf.PerlinNoise(x / 10f + MAP_LIMIT, y / 10f + MAP_LIMIT);
                int index = noise < 0.6f ? 0 : 1;

                Terrain.SetTile(new Vector3Int(x, y, -2), terrain_tiles[index]);
                //Walls.SetTile(new Vector3Int(x, y, 0), terrain_tiles[0]);
            }
            yield return null;
        }
        //AstarPath.UpdateGraphs(new Bounds(new Vector3(0, 0, 0), new Vector3(MAP_LIMIT, MAP_LIMIT, MAP_LIMIT)));
    }
    private void InitializeMap()
    {
        
        _mapInitialized = true;
        int ROOM_LIMIT = 10;
        Debug.Log("All walls loaded.");
        Debug.Log("All terrains loaded.");
        Walls.ClearAllEditorPreviewTiles();
        for (int x = 0; x < ROOM_LIMIT; x++)
        {
            TileWall wallTop = ScriptableObject.CreateInstance<TileWall>();
            TileWall wallBottom = ScriptableObject.CreateInstance<TileWall>();

            wallTop.wallName = WALL_NAME;
            wallBottom.wallName = WALL_NAME;

            Walls.SetTile(new Vector3Int(x, 0, -2), wallTop);
            Walls.SetTile(new Vector3Int(x, ROOM_LIMIT, -2), wallBottom);
        }
        for (int y = 0; y < ROOM_LIMIT; y++)
        {
            TileWall wallLeft = ScriptableObject.CreateInstance<TileWall>();
            TileWall wallRight = ScriptableObject.CreateInstance<TileWall>();

            wallLeft.wallName = WALL_NAME;
            wallRight.wallName = WALL_NAME;

            Walls.SetTile(new Vector3Int(0, y, -2), wallLeft);
            Walls.SetTile(new Vector3Int(ROOM_LIMIT, y, -2), wallRight);
        }
        for (int x = 30; x < ROOM_LIMIT + 30; x++)
            for (int y = 0; y < ROOM_LIMIT; y++)
            {
                TileWall wall = ScriptableObject.CreateInstance<TileWall>();
                wall.wallName = WALL_NAME;
                Walls.SetTile(new Vector3Int(x, y, -2), wall);
            }
        StartCoroutine("LoadTerrain");
    }
    private void Update()
    {
        if (WallSprites.Count != _maxWallTiles || TerrainSprites.Count != _maxTerrainTiles)
            return;
        if (!_mapInitialized)
            InitializeMap();
    }

    public Vector3Int GetMousePosition(Vector2 mousePosition)
    {
        return Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(mousePosition));
    }
    public bool BuildWall(Vector3Int coordinates)
    {
        if (Walls.HasTile(coordinates))
            return false;
        //AstarPath.UpdateGraphs(new Bounds(coordinates, new Vector3(2, 2, 2)));
        TileWall wall = ScriptableObject.CreateInstance<TileWall>();
        wall.wallName = WALL_NAME;
        Walls.SetTile(coordinates, wall);
        //Walls.RefreshAllTiles();
        return true;
    }
    public bool DestroyWall(Vector3Int coordinates)
    {
        if (!Walls.HasTile(coordinates))
            return false;
        //AstarPath.UpdateGraphs(new Bounds(coordinates, new Vector3(2, 2, 2)));
        Walls.SetTile(coordinates, null);
        BuildTerrain(coordinates, TERRAIN_AFTER_WALL_DECON);
        //Walls.RefreshAllTiles();
        return true;
    }

    public bool BuildTerrain(Vector3Int coordinates, string name)
    {
        if (Walls.HasTile(coordinates))
            return false;
        Tile wall = ScriptableObject.CreateInstance<Tile>();
        wall.sprite = TerrainSprites[name];
        Terrain.SetTile(coordinates, wall);
        //Terrain.RefreshAllTiles();
        return true;
    }
    //public bool BuildFurniture(Vector3Int coordinates, string name)
    //{
    //    if (Walls.HasTile(coordinates))
    //        return false;
    //    Tile wall = ScriptableObject.CreateInstance<Tile>();
    //    wall.sprite = TerrainSprites[name];
    //    Terrain.SetTile(coordinates, wall);
    //    //Terrain.RefreshAllTiles();
    //    return true;
    //}
    static public Sprite GetWallSpriteByName(string name)
    {
        if (WallSprites.ContainsKey(name + "_"))
            return WallSprites[name + "_"];
        Debug.Log("Wall Sprite not found: " + name + "_");
        return null;
    }
    static public Sprite GetTerrainSpriteByName(string name)
    {
        if (TerrainSprites.ContainsKey(name))
            return TerrainSprites[name];
        Debug.Log("Terrain Sprite not found: " + name);
        return null;
    }
}
