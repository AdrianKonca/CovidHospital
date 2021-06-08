using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        _previewSpriteRenderer = buildingController.preview.GetComponent<SpriteRenderer>();
        buildingController.SetBuildingUIController(this);
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
        furnitureName = furnitureName.Replace(" ", "");
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
