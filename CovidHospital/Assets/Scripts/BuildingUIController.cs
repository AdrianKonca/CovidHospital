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
    private SpriteRenderer _previewSpriteRenderer;
    void Awake()
    {
        _previewSpriteRenderer = buildingController.preview.GetComponent<SpriteRenderer>();
        buildingController.SetBuildingUIController(this);
    }
    public void OnBuildWallButtonClicked()
    {
        _previewSpriteRenderer.sprite = MapController.GetWallSpriteByName("ConcreteWall");
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
        
        _previewSpriteRenderer.sprite = MapController.GetTerrainSpriteByName("Concrete");
        buildingController.SetState(
            BuildingController.State.BuildTerrain
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
