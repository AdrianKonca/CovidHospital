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

    private Dictionary<Vector3Int, float> _recentErrors = new Dictionary<Vector3Int, float>();
    private void Build()
    {
        if (!_actionStarted)
            return;
        (bool, string) response;
        var position = _mapController.GetMousePosition(_mousePosition);

        switch (_state)
        {
            case State.Inactive:
                return;
            case State.BuildTerrain:
                _mapController.BuildTerrain(position, "Concrete");
                break;
            case State.BuildWall:
                response = _mapController.BuildWall(position);
                if (!response.Item1 && !_recentErrors.ContainsKey(position))
                {
                    _recentErrors[position] = Time.time + 5f;
                    FloatingTextManager.I().DisplayText(response.Item2, position, Color.red);
                }
                else
                    _recentErrors[position] = Time.time + 2f;
                break;
            case State.DestroyWall:
                _mapController.DestroyWall(position);
                break;
            case State.BuildFurniture:
                response = _mapController.BuildFurniture(position, CurrentObjectName, rotations[_rotation]);
                if (!response.Item1 && !_recentErrors.ContainsKey(position))
                {
                    _recentErrors[position] = Time.time + 5f;
                    FloatingTextManager.I().DisplayText(response.Item2, position, Color.red);
                }
                else
                {
                    _recentErrors[position] = Time.time + 2f;
                }
                if (response.Item1)
                {
                    _uiController.UpdateSelectionName(CurrentObjectName);
                }
                _actionStarted = false;
                break;
            case State.DestroyFurniture:
                var values = _mapController.DestroyFurniture(position);
                _actionStarted = false;
                break;
        }
    }

    public void SetState(State state)
    {
        _state = state;
        _rotation = 0;
    }
    public State GetState()
    {
        return _state;
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
        var keysToRemove = new List<Vector3Int>();
        foreach (var key in _recentErrors.Keys)
        {
            if (_recentErrors[key] < Time.time)
                keysToRemove.Add(key);
        }
        foreach (var key in keysToRemove)
        {
            _recentErrors.Remove(key);
        }
    }
    public void SetBuildingUIController(BuildingUIController controller)
    {
        _uiController = controller;
    }
}
