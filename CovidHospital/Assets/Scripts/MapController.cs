using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

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
    private int maxWallTiles = 0;
    // Start is called before the first frame update
    
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
        string[] directions = { "", "E", "ES", "ESW", "EW", "N", "NE", "NES", "NESW", "NEW", "NS", "NSW", "NW", "S", "SW", "W"};
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
        maxWallTiles = directions.Length * wallNames.Length;
    }

    private bool _mapInitialized = false;

    string WALL_NAME = "ConcreteWall";
    private void InitializeMap()
    {
        _mapInitialized = true;
        int ROOM_LIMIT = 10;
        Debug.Log("All walls loaded.");
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
    }
    private void Update()
    {
        if (WallSprites.Count != maxWallTiles)
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
        TileWall wall = ScriptableObject.CreateInstance<TileWall>();
        wall.wallName = WALL_NAME;
        Walls.SetTile(coordinates, wall);
        Walls.RefreshAllTiles();
        return true;
    }
    public bool DestroyWall(Vector3Int coordinates)
    {
        TileWall wall = ScriptableObject.CreateInstance<TileWall>();
        wall.wallName = WALL_NAME;
        Walls.SetTile(coordinates, null);
        Walls.RefreshAllTiles();
        return true;
    }
}
