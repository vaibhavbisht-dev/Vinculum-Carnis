using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHead : MonoBehaviour
{
    [Header("Player View Setting")]
    [SerializeField, Range(1, 100)] private int _mouseSensitivity = 100;
    [SerializeField] private Transform _playerBody;
    

    float _xRotation = 0f;


    private void Start()
    {
        _playerBody = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Camera.main.transform.SetParent(transform, false);
        Camera.main.transform.SetPositionAndRotation(transform.position,transform.rotation);
    }

    private void Update()
    {
        LookAround();
    }

    private void LookAround()
    {
        Vector2 mouseInput = _mouseSensitivity * Time.deltaTime * Mouse.current.delta.ReadValue();
        _xRotation -= mouseInput.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerBody.Rotate(Vector3.up * mouseInput.x);
    }
}
