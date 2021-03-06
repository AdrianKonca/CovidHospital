using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// TODO: Refactor
// TODO: Make camera movement less snappy
// TODO: Add panning by holding cursor near screen border.
public class CameraController : MonoBehaviour
{
    public float MaxZoom = 40;
    public float MinZoom = 5;
    public float ZoomSensitivity = 1f;
    public float PanningSensitivity = 6f;
    public Vector3 direction;
    private Camera _camera;
    private Controls _controls;
    private PlayerInput _playerInput;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _playerInput = FindObjectOfType<PlayerInput>();
        _controls = new Controls();
        _playerInput.onActionTriggered += onActionTrigered;
    }

    // Update is called once per frame
    private void Update()
    {
        if (direction.magnitude > 0)
            _camera.transform.position =
                _camera.transform.position + direction *
                (Time.deltaTime * PanningSensitivity) *
                (EaseInQuad(_camera.orthographicSize / MaxZoom) * 3 + 0.75f);
    }

    //TODO: move to its own file
    public static float EaseInQuad(float value)
    {
        return value * value;
    }


    // for future self started -> performed -> canceled
    private void onActionTrigered(InputAction.CallbackContext action)
    {
        var isPointerOverGui = EventSystem.current.IsPointerOverGameObject();

        if (action.action.name == _controls.Player.CameraZoom.name && action.performed && !isPointerOverGui)
        {
            var mouseMovement = action.ReadValue<Vector2>().y * ZoomSensitivity;
            var newCameraZoom = _camera.orthographicSize - mouseMovement;
            _camera.orthographicSize = Mathf.Clamp(newCameraZoom, MinZoom, MaxZoom);
        }
        else if (action.action.name == _controls.Player.CameraPan.name
                 && (action.performed && !isPointerOverGui
                     || action.canceled))
        {
            //Debug.Log(string.Format("{0} {1} {2} {3}", action.action.name, action.started, action.performed, action.canceled));
            //Debug.Log(string.Format("{0} {1} {2} {3}", action.ReadValue<Vector2>(), action.started, action.performed, action.canceled));
            var cameraOffset = action.ReadValue<Vector2>() * PanningSensitivity;
            direction = new Vector3(cameraOffset.x, cameraOffset.y, 0);
        }
    }
}