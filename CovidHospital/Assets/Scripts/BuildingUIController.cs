using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIController : MonoBehaviour
{
    public BuildingController buildingController;

    public void OnBuildWallButtonClicked()
    {
        buildingController.SetState(
            BuildingController.State.BuildWall
        );
    }

    public void OnDestroyWallButtonClicked()
    {
        buildingController.SetState(
            BuildingController.State.DestroyWall
        );
    }

    public void OnBuildTerrainButtonClicked()
    {
        buildingController.SetState(
            BuildingController.State.BuildTerrain
        );
    }

    public void OnWallSelected()
    {

    }

    
}
