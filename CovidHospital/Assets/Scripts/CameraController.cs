using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Refactor
// TODO: Make camera movement less snappy
// TODO: Add panning by holding cursor near screen border.
public class CameraController : MonoBehaviour
{
    Camera _camera;
    Controls _controls;
    PlayerInput _playerInput;
    public float MaxZoom = 40;
    public float MinZoom = 5;
    public float ZoomSensitivity = 1f;
    public float PanningSensitivity = 6f;
    public Vector3 direction = new Vector3();

    //TODO: move to its own file
    public static float EaseInQuad(float value)
    {
        return value * value;
    }
    void Start()
    {
        _camera = GetComponent<Camera>();
        _playerInput = FindObjectOfType<PlayerInput>();
        _controls = new Controls();
        _playerInput.onActionTriggered += onActionTrigered;   
    }

    
    // for future self started -> performed -> canceled
    private void onActionTrigered(InputAction.CallbackContext action)
    {
        
        if (action.action.name == _controls.Player.CameraZoom.name && action.performed)
        {
            var mouseMovement = action.ReadValue<Vector2>().y * ZoomSensitivity;
            var newCameraZoom = _camera.orthographicSize - mouseMovement;
            _camera.orthographicSize = Mathf.Clamp(newCameraZoom, MinZoom, MaxZoom);
        }
        else if (action.action.name == _controls.Player.CameraPan.name && (action.performed || action.canceled))
        {
            //Debug.Log(string.Format("{0} {1} {2} {3}", action.action.name, action.started, action.performed, action.canceled));
            //Debug.Log(string.Format("{0} {1} {2} {3}", action.ReadValue<Vector2>(), action.started, action.performed, action.canceled));
            var cameraOffset = action.ReadValue<Vector2>() * PanningSensitivity;
            direction = new Vector3(cameraOffset.x, cameraOffset.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (direction.magnitude > 0)
            _camera.transform.position = 
                _camera.transform.position + direction * 
                    (Time.deltaTime * PanningSensitivity) * 
                    (EaseInQuad(_camera.orthographicSize / MaxZoom) * 3 + 0.75f);
    }
}
