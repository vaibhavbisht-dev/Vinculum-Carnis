using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private Raycaster _raycaster;
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private Transform _playerAnimalHolder;

    private void Start()
    {
        _raycaster = GetComponent<Raycaster>();
        _interactAction.action.Enable();
    }
    private void OnDestroy()
    {
        _interactAction.action.Disable();
    }

    private void Update()
    {
        Interact();
    }

    private void Interact()
    {
        if (_raycaster.IsHitting && _interactAction.action.WasPressedThisFrame())
        {
            if (_raycaster.Hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnInteract();
            }
        }
        if(_raycaster.IsHitting && _interactAction.action.WasPressedThisFrame())
        {
            if (_raycaster.Hit.collider.TryGetComponent(out IInteractableAnimal interactableAnimal))
            {
                interactableAnimal.OnInteract(_playerAnimalHolder);
            }
        }
    }
}
