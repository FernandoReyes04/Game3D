using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Camera Position")]
    public float distance = 2f;
    public float heightOffset = 1f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 1f;
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 70f;

    [Header("Camera Target Point")]
    public float lookAtHeightOffset = 1.5f;

    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Camera automatically found Player as target!");
            }
            else
            {
                Debug.LogWarning("No target found for camera!");
                return;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        horizontalAngle = angles.y;
        verticalAngle = angles.x;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            horizontalAngle += mouseDelta.x * mouseSensitivity * 0.1f;
            verticalAngle -= mouseDelta.y * mouseSensitivity * 0.1f;

            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);

        Vector3 offset = rotation * new Vector3(0, heightOffset, -distance);

        transform.position = target.position + offset;

        Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
        transform.LookAt(lookAtPoint);
    }
}
