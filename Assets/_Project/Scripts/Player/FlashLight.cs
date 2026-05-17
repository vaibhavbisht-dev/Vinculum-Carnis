using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] private Light _light;
    [SerializeField] private InputActionReference _toggleFlashlightAction;

    private void Start()
    {
        _toggleFlashlightAction.action.Enable();
        if (_light == null) _light = GetComponent<Light>();
        _light.enabled = false; // Start with flashlight off
    }

    private void OnDestroy()
    {
        _toggleFlashlightAction.action.Disable();
    }

    private void Update()
    {
        ToggleFlashLight();
    }


    private void ToggleFlashLight() { 
        if(_toggleFlashlightAction.action.triggered)
        {
            _light.enabled = !_light.enabled;
        }
    }
}
