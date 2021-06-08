using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIController : MonoBehaviour
{
    public enum SelectionType
    {
        Default, Deconstruction
    }

    public BuildingController buildingController;
    public Sprite defaultSprite;
    public Sprite deconstructionSprite;
    public Dropdown FurnitureSelection;
    private SpriteRenderer _previewSpriteRenderer;
    private MapController _mapController;
    private string FormatControlName(string name)
    {
        if (_mapController.FurnituresLimit.ContainsKey(name))
        {
            var limit = _mapController.FurnituresLimit[name];
            return $"{name} {limit.Item1}/{limit.Item2}";
        }
        else
            return name;

    }
    void Start()
    {
        _mapController = MapController.Instance();
        _previewSpriteRenderer = buildingController.preview.GetComponent<SpriteRenderer>();
        buildingController.SetBuildingUIController(this);
        var options = new List<Dropdown.OptionData>();
        foreach (var name in _mapController.FurnituresNames)
        {
            var option = new Dropdown.OptionData();
            option.text = FormatControlName(name);
            options.Add(option);
        }
        FurnitureSelection.options = options;
        FurnitureSelection.onValueChanged.AddListener(delegate {
            DropdownValueChanged();
        });
    }
    void DropdownValueChanged()
    {
        if (buildingController.GetState() == BuildingController.State.BuildFurniture)
        {
            OnBuildFurnitureButtonClicked();
        }
    }

    public void OnBuildWallButtonClicked()
    {
        _previewSpriteRenderer.sprite = SpriteManager.GetWallSpriteByName("ConcreteWall");
        buildingController.SetState(
            BuildingController.State.BuildWall
        );
    }

    public void OnDestroyWallButtonClicked()
    {
        SetSelectionSprite(SelectionType.Deconstruction);
        buildingController.SetState(
            BuildingController.State.DestroyWall
        );
    }

    public void OnBuildTerrainButtonClicked()
    {
        _previewSpriteRenderer.sprite = SpriteManager.GetTerrainSpriteByName("Concrete");
        buildingController.SetState(
            BuildingController.State.BuildTerrain
        );
    }
    public void OnBuildFurnitureButtonClicked()
    {
        
        string furnitureName = FurnitureSelection.options[FurnitureSelection.value].text;

        furnitureName = Regex.Replace(furnitureName, @"[0-9 \/]", "");
        UpdateFurnitureSprite(furnitureName, "N");
        buildingController.CurrentObjectName = furnitureName;
        buildingController.SetState(
            BuildingController.State.BuildFurniture
        );
        
    }
    public void UpdateFurnitureSprite(string name, string rotation)
    {
        var rotations = new Dictionary<string, int>{
            { "N", 0 },
            { "E", -90 },
            { "S", -180 },
            { "W", -270 },
        };
        _previewSpriteRenderer.sprite = SpriteManager.GetFurnitureSpriteByName(name);
        _previewSpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, rotations[rotation]);
    }

    internal void UpdateSelectionName(string currentObjectName)
    {
        FormatControlName(currentObjectName);
        foreach (var option in FurnitureSelection.options)
        {
            if (Regex.IsMatch(option.text, $"^{currentObjectName}[ 0-9]*"))
            {

                FurnitureSelection.captionText.text = FormatControlName(currentObjectName);
                option.text = FormatControlName(currentObjectName);
            }
        }
    }

    public void OnDestroyFurnitureButtonClicked()
    {

        SetSelectionSprite(SelectionType.Deconstruction);
        buildingController.SetState(
            BuildingController.State.DestroyFurniture
        );

    }

    public void SetSelectionSprite(SelectionType type)
    {
        switch (type)
        {
            case SelectionType.Default:
                _previewSpriteRenderer.sprite = defaultSprite;
                break;
            case SelectionType.Deconstruction:
                _previewSpriteRenderer.sprite = deconstructionSprite;
                break;
            default:
                Debug.LogWarning("No type of selection passed");
                break;
        }
    }
}
