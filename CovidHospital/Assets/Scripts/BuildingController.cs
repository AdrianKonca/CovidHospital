using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingController : MonoBehaviour
{
    public GameObject preview;
    public string CurrentObjectName = null;
    public enum State
    {
        Inactive,
        BuildTerrain,
        BuildWall,
        DestroyWall,
        BuildFurniture,
        DestroyFurniture,
    }

    Controls _controls;
    PlayerInput _playerInput;
    MapController _mapController;
    BuildingUIController _uiController;

    int _rotation = 0;
    Vector2 _mousePosition = new Vector2();
    bool _actionStarted = false;
    State _state = State.Inactive;

    void Start()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _mapController = FindObjectOfType<MapController>();
        _controls = new Controls();
        _playerInput.onActionTriggered += onActionTrigered;
    }

    Dictionary<int, string> rotations = new Dictionary<int, string>() {
        { 0, "N" },
        { 1, "E" },
        { 2, "S" },
        { 3, "W" },
    };
    private void Build()
    {
        if (!_actionStarted)
            return;
        switch (_state)
        {
            case State.Inactive:
                return;
            case State.BuildTerrain:
                _mapController.BuildTerrain(_mapController.GetMousePosition(_mousePosition), "Concrete");
                break;
            case State.BuildWall:
                _mapController.BuildWall(_mapController.GetMousePosition(_mousePosition));
                break;
            case State.DestroyWall:
                _mapController.DestroyWall(_mapController.GetMousePosition(_mousePosition));
                break;
            case State.BuildFurniture:
                _mapController.BuildFurniture(_mapController.GetMousePosition(_mousePosition), CurrentObjectName, rotations[_rotation]);
                _actionStarted = false;
                break;
            case State.DestroyFurniture:
                var values = _mapController.DestroyFurniture(_mapController.GetMousePosition(_mousePosition));
                _actionStarted = false;
                break;
        }
    }

    public void SetState(State state)
    {
        _state = state;
        _rotation = 0;
    }
    private void onActionTrigered(InputAction.CallbackContext action)
    {
        
        if (action.action.name == _controls.Player.MoveMouse.name && action.performed)
        {
            _mousePosition = action.ReadValue<Vector2>();
            preview.transform.position = _mapController.GetMousePosition(_mousePosition) + new Vector3(0.5f, 0.5f);
            Build();
        }
        else if (action.action.name == _controls.Player.Action.name)
        {
            //don't ask why...
            bool isPointerOverUIElement = EventSystem.current.IsPointerOverGameObject();
            if (action.started && !isPointerOverUIElement)
                _actionStarted = true;
            else if (action.canceled)
                _actionStarted = false;
        }
        else if (action.action.name == _controls.Player.Cancel.name)
        {
            _uiController.SetSelectionSprite(BuildingUIController.SelectionType.Default);
            _state = State.Inactive;
        }
        else if (action.action.name == _controls.Player.Rotate.name && action.performed)
        {
            _rotation = (_rotation + 1) % 4;
            _uiController.UpdateFurnitureSprite(CurrentObjectName, rotations[_rotation]);
        }
    }

    private void Update()
    {
        var pos = _mapController.GetMousePosition(_mousePosition);

        preview.transform.position = pos + new Vector3(0.5f, 0.5f);

        Build();
    }
    public void SetBuildingUIController(BuildingUIController controller)
    {
        _uiController = controller;
    }
}
