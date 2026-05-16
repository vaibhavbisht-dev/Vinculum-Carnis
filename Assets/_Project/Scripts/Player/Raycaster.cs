using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10.0f;
    [SerializeField] private LayerMask interactableLayer; // Set this in the Inspector

    private Camera cam;
    public RaycastHit Hit { get; private set; }
    public bool IsHitting { get; private set; }

    bool lastFrameHit = false;

    private void Start()
    {
        // Cache the camera reference for better performance
        cam = Camera.main;
    }

    private void Update()
    {
        CastARay();
        if (lastFrameHit != IsHitting) {
            lastFrameHit = IsHitting;
            // UI manager Added Later
        }
    }

    private void CastARay()
    {
        // Center of screen (0.5, 0.5)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Use the LayerMask to ignore things like the player's own collider
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayer))
        {
            Hit = hit;

            IsHitting = true;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            Hit = default;
            IsHitting = false;
        }
    }

}
