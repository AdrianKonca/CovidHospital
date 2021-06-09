using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        var spriteName = wallName + "_";
        string[] directions = {"N", "E", "S", "W"};
        var neighobrs = new Dictionary<string, Vector3Int>
        {
            {"N", new Vector3Int(position.x, position.y + 1, position.z)},
            {"E", new Vector3Int(position.x + 1, position.y, position.z)},
            {"S", new Vector3Int(position.x, position.y - 1, position.z)},
            {"W", new Vector3Int(position.x - 1, position.y, position.z)}
        };
        foreach (var direction in directions)
            if (IsNeighbour(neighobrs[direction], tilemap))
            {
                spriteName += direction;
                if (continueToNeighbors)
                    SelectNewSprite(neighobrs[direction], tilemap, false);
            }

        if (!continueToNeighbors)
        {
            var me = tilemap.GetTile<TileWall>(position);
            me.sprite = SpriteManager.WallSprites[spriteName];
        }
        else
        {
            sprite = SpriteManager.WallSprites[spriteName];
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
        var tile = tilemap.GetTile(position);
        return tile != null;
    }
}

public class MapController : MonoBehaviour
{
    public delegate void OnFurnitureBuiltDelegate(GameObject furniture);

    private static MapController _instance;
    public AstarPath AstarPath;
    public List<GameObject> FurnituresPrefabs;

    public List<string> FurnituresNames = new List<string>
    {
        "Bed", "BedOxygen", "BedRespirator", "BedsideLamp", "Chair", "Fotel", "Shower", "Sofa", "Toilet"
    };

    public Tilemap Terrain;
    public Tilemap Walls;

    private GameObject _furnitures;

    private bool _mapInitialized;
    private readonly int DEFAULT_HEIGHT_Z = -2;
    private readonly Vector3 FURNITURE_OFFSET = new Vector3(0.5f, 0.5f, 0f);
    public Dictionary<string, List<GameObject>> Furnitures = new Dictionary<string, List<GameObject>>();

    public Dictionary<string, (int, int)> FurnituresLimit = new Dictionary<string, (int, int)>
    {
        {"BedOxygen", (0, 10)},
        {"BedRespirator", (0, 5)}
    };

    //used for building colission detection
    public Dictionary<Vector3Int, GameObject> FurnituresMap = new Dictionary<Vector3Int, GameObject>();

    //used for pathfinding
    public Dictionary<Vector3Int, GameObject> FurnituresUnique = new Dictionary<Vector3Int, GameObject>();

    public Dictionary<string, GameObject> NameToFurniture = new Dictionary<string, GameObject>();
    private readonly string TERRAIN_AFTER_WALL_DECON = "Concrete";
    private readonly string WALL_NAME = "ConcreteWall";

    private void Awake()
    {
        if (_instance != null) Debug.LogError("One map controller already exists.");

        _furnitures = new GameObject("Furnitures");
        _furnitures.transform.parent = transform;
        _instance = this;
    }

    private void Update()
    {
        if (!SpriteManager.AllSpritesLoaded)
            return;
        if (!_mapInitialized)
            InitializeMap();
    }

    public event OnFurnitureBuiltDelegate OnFurnitureBuilt;

    public static MapController Instance()
    {
        return _instance;
    }

    private IEnumerator LoadTerrain()
    {
        var MAP_LIMIT = 100;

        var terrain_names = new List<string> {"Grass", "Dirt", "Concrete"};
        var terrain_tiles = new List<Tile>();
        foreach (var name in terrain_names)
        {
            var terrain = ScriptableObject.CreateInstance<Tile>();
            terrain.sprite = SpriteManager.TerrainSprites[name];
            terrain_tiles.Add(terrain);
        }

        for (var x = -MAP_LIMIT; x < MAP_LIMIT; x++)
        {
            for (var y = -MAP_LIMIT; y < MAP_LIMIT; y++)
            {
                var noise = Mathf.PerlinNoise(x / 10f + MAP_LIMIT, y / 10f + MAP_LIMIT);
                var index = noise < 0.6f ? 0 : 1;
                Terrain.SetTile(new Vector3Int(x, y, DEFAULT_HEIGHT_Z), terrain_tiles[index]);
            }

            yield return null;
        }

        terrain_tiles[2].name = "Concrete";
        for (var x = -10; x <= 10; x++)
        for (var y = -10; y <= 10; y++)
            Terrain.SetTile(new Vector3Int(x, y, DEFAULT_HEIGHT_Z), terrain_tiles[2]);
        AstarPath.UpdateGraphs(new Bounds(new Vector3(0, 0, 0), new Vector3(MAP_LIMIT, MAP_LIMIT, MAP_LIMIT)));
    }

    private void InitializeMap()
    {
        _mapInitialized = true;
        var ROOM_LIMIT = 10;
        Walls.ClearAllEditorPreviewTiles();

        //for (int x = 0; x < ROOM_LIMIT; x++)
        //{
        //    TileWall wallTop = ScriptableObject.CreateInstance<TileWall>();
        //    TileWall wallBottom = ScriptableObject.CreateInstance<TileWall>();

        //    wallTop.wallName = WALL_NAME;
        //    wallBottom.wallName = WALL_NAME;

        //    Walls.SetTile(new Vector3Int(x, 0, DEFAULT_HEIGHT_Z), wallTop);
        //    Walls.SetTile(new Vector3Int(x, ROOM_LIMIT, DEFAULT_HEIGHT_Z), wallBottom);
        //}

        //for (int y = 0; y < ROOM_LIMIT; y++)
        //{
        //    TileWall wallLeft = ScriptableObject.CreateInstance<TileWall>();
        //    TileWall wallRight = ScriptableObject.CreateInstance<TileWall>();

        //    wallLeft.wallName = WALL_NAME;
        //    wallRight.wallName = WALL_NAME;

        //    Walls.SetTile(new Vector3Int(0, y, DEFAULT_HEIGHT_Z), wallLeft);
        //    Walls.SetTile(new Vector3Int(ROOM_LIMIT, y, DEFAULT_HEIGHT_Z), wallRight);
        //}
        //for (int x = 30; x < ROOM_LIMIT + 30; x++)
        //    for (int y = 0; y < ROOM_LIMIT; y++)
        //    {
        //        TileWall wall = ScriptableObject.CreateInstance<TileWall>();
        //        wall.wallName = WALL_NAME;
        //        Walls.SetTile(new Vector3Int(x, y, DEFAULT_HEIGHT_Z), wall);
        //    }

        StartCoroutine("LoadTerrain");

        //move to other function
        foreach (var furniture in FurnituresPrefabs)
        {
            Debug.Log(furniture.name);
            NameToFurniture[furniture.name] = furniture;
            Furnitures[furniture.name] = new List<GameObject>();
        }
    }

    public Vector3Int GetMousePosition(Vector2 mousePosition)
    {
        return Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(mousePosition));
    }

    public (bool, string) BuildWall(Vector3Int coordinates)
    {
        if (Walls.HasTile(coordinates))
            return (false, "Wall is already built here!");
        if (FurnituresMap.ContainsKey(coordinates))
            return (false, "You can't build walls over furnitures!");
        AstarPath.UpdateGraphs(new Bounds(coordinates, new Vector3(2, 2, 2)));
        var wall = ScriptableObject.CreateInstance<TileWall>();
        wall.wallName = WALL_NAME;
        Walls.SetTile(coordinates, wall);
        return (true, string.Empty);
    }

    public bool DestroyWall(Vector3Int coordinates)
    {
        if (!Walls.HasTile(coordinates))
            return false;
        AstarPath.UpdateGraphs(new Bounds(coordinates, new Vector3(2, 2, 2)));
        Walls.SetTile(coordinates, null);
        BuildTerrain(coordinates, TERRAIN_AFTER_WALL_DECON);
        return true;
    }

    public bool BuildTerrain(Vector3Int coordinates, string name)
    {
        if (Walls.HasTile(coordinates))
            return false;
        var wall = ScriptableObject.CreateInstance<Tile>();
        wall.sprite = SpriteManager.TerrainSprites[name];
        wall.name = name;
        Terrain.SetTile(coordinates, wall);
        return true;
    }

    private List<Vector3Int> GetFurnitePositions(FurnitureController furnitureController, Vector3Int coordinates,
        string rotation)
    {
        var points = new List<Vector3Int>();
        switch (rotation)
        {
            case "N":
                for (var x = 0; x < furnitureController.size.x; x++)
                for (var y = 0; y < furnitureController.size.y; y++)
                    points.Add(new Vector3Int(coordinates.x + x, coordinates.y - y, DEFAULT_HEIGHT_Z));
                break;
            case "S":
                for (var x = -furnitureController.size.x + 1; x <= 0; x++)
                for (var y = 0; y < furnitureController.size.y; y++)
                    points.Add(new Vector3Int(coordinates.x + x, coordinates.y + y, DEFAULT_HEIGHT_Z));
                break;
            case "W":
                for (var x = 0; x < furnitureController.size.y; x++)
                for (var y = 0; y < furnitureController.size.x; y++)
                    points.Add(new Vector3Int(coordinates.x + x, coordinates.y + y, DEFAULT_HEIGHT_Z));
                break;
            case "E":
                for (var x = -furnitureController.size.y + 1; x <= 0; x++)
                for (var y = 0; y < furnitureController.size.x; y++)
                    points.Add(new Vector3Int(coordinates.x + x, coordinates.y - y, DEFAULT_HEIGHT_Z));
                break;
        }

        return points;
    }

    public (bool, string) BuildFurniture(Vector3Int coordinates, string name, string rotation)
    {
        var rotations = new Dictionary<string, int>
        {
            {"N", 0},
            {"E", -90},
            {"S", -180},
            {"W", -270}
        };

        var furniture = Instantiate(NameToFurniture[name]);
        var furnitureController = furniture.GetComponent<FurnitureController>();

        var points = GetFurnitePositions(furnitureController, coordinates, rotation);

        foreach (var point in points)
        {
            if (Walls.HasTile(point))
            {
                Destroy(furniture);
                return (false, "There is a wall here.");
            }

            if (FurnituresMap.ContainsKey(point))
            {
                Destroy(furniture);
                return (false, "There is another furniture here.");
            }

            if (Terrain.HasTile(point) && Terrain.GetTile(point).name != "Concrete")
            {
                Destroy(furniture);
                return (false, "The floor must be made of concrete!");
            }
        }

        if (FurnituresLimit.ContainsKey(name))
        {
            if (FurnituresLimit[name].Item1 == FurnituresLimit[name].Item2)
            {
                Destroy(furniture);
                return (false, $"You can't build more {name}!");
            }

            FurnituresLimit[name] = (FurnituresLimit[name].Item1 + 1, FurnituresLimit[name].Item2);
        }

        foreach (var point in points) FurnituresMap[point] = furniture;

        FurnituresUnique[coordinates] = furniture;
        Furnitures[name].Add(furniture);

        furniture.name = name;
        furniture.transform.rotation = Quaternion.Euler(0, 0, rotations[rotation]);
        furniture.transform.position = coordinates + FURNITURE_OFFSET;
        furniture.transform.parent = _furnitures.transform;
        furnitureController.pointsTaken = points;
        OnFurnitureBuilt?.Invoke(furniture);
        return (true, string.Empty);
    }

    public (bool, string) DestroyFurniture(Vector3Int coordinates)
    {
        if (!FurnituresMap.ContainsKey(coordinates))
            return (false, "No objects to destroy!");
        var furniture = FurnituresMap[coordinates];
        var furnitureController = furniture.GetComponent<FurnitureController>();

        foreach (var point in furnitureController.pointsTaken)
            FurnituresMap.Remove(point);
        FurnituresUnique.Remove(coordinates);
        Furnitures[furniture.name].Remove(furniture);
        Destroy(furniture);

        //TOOD: Add handling for furniture owners.
        return (true, string.Empty);
    }

    public List<GameObject> GetFurnitures(string name)
    {
        return Furnitures[name];
    }

    public GameObject GetClosestFurniture(string name, Vector3 coordinates)
    {
        return Furnitures[name]
            .OrderBy(f => Vector3.Distance(f.transform.position, coordinates))
            .FirstOrDefault();
    }

    public GameObject GetClosestFreeFurniture(string name, Vector3 coordinates)
    {
        return Furnitures[name]
            .Where(f => f.GetComponent<FurnitureController>().owner == null)
            .OrderBy(f => Vector3.Distance(f.transform.position, coordinates))
            .FirstOrDefault();
    }

    public GameObject GetClosestFreeBed(Vector3 coordinates)
    {
        var beds = Furnitures["Bed"].Where(f => f.GetComponent<FurnitureController>().owner == null).ToList();
        var oBeds = Furnitures["BedOxygen"].Where(f => f.GetComponent<FurnitureController>().owner == null).ToList();
        var rBeds = Furnitures["BedRespirator"].Where(f => f.GetComponent<FurnitureController>().owner == null)
            .ToList();
        beds.AddRange(oBeds);
        beds.AddRange(rBeds);
        return beds
            .OrderBy(f => Vector3.Distance(f.transform.position, coordinates))
            .FirstOrDefault();
    }
}