using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    private InputAction mouseAction;
    public float xSensitivity;
    public float ySensitivity;
    float xRotation;
    float yRotation;
    public Transform playerOrientation;

    private void Awake()
    {
        mouseAction = new InputAction("Look", InputActionType.Value);
        mouseAction.AddBinding("<Mouse>/delta");
    }

    private void OnEnable()
    {
        mouseAction.Enable();
    }

    private void OnDisable()
    {
        mouseAction.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = mouseAction.ReadValue<Vector2>().x * Time.deltaTime * xSensitivity;
        float mouseY = mouseAction.ReadValue<Vector2>().y * Time.deltaTime * ySensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

}
