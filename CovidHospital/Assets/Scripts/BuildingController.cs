using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingController : MonoBehaviour
{
    Controls _controls;
    PlayerInput _playerInput;
    MapController _mapController;

    Vector2 _mousePosition = new Vector2();
    bool _build = false;
    bool _destroy = false;
    void Start()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _mapController = FindObjectOfType<MapController>();
        _controls = new Controls();
        _playerInput.onActionTriggered += onActionTrigered;
    }

    private void onActionTrigered(InputAction.CallbackContext action)
    {
        if (action.action.name == _controls.Player.MoveMouse.name && action.performed)
        {
            _mousePosition = action.ReadValue<Vector2>();
        }
        else if (action.action.name == _controls.Player.Action.name)
        {
            if (action.started)
                _build = true;
            else if (action.canceled)
                _build = false;
        }
        else if (action.action.name == _controls.Player.Cancel.name)
        {
            if (action.started)
                _destroy = true;
            else if (action.canceled)
                _destroy = false;
        }
    }

    private void Update()
    {
        if (_build && !_destroy)
            _mapController.BuildWall(_mapController.GetMousePosition(_mousePosition));
        if (_destroy && !_build)
            _mapController.DestroyWall(_mapController.GetMousePosition(_mousePosition));
    }
}
