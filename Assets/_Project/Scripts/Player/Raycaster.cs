using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] private float maxDistanceInteraction = 10.0f;
    [SerializeField] private float maxDistanceBulletHit = 50.0f;
    [SerializeField] private LayerMask interactableLayer; // Set this in the Inspector
    [SerializeField] private LayerMask animalLayer; // Set this in the Inspector

    private Camera cam;
    public RaycastHit Hit { get; private set; }
    public RaycastHit HitAnimal { get; private set; }
    public bool IsHitting { get; private set; }
    public bool IsHittingAnimal { get; private set; }

    bool lastFrameHit = false;
    bool lastFrameHitAnimal = false;

    private void Start()
    {
        // Cache the camera reference for better performance
        cam = Camera.main;
    }

    private void Update()
    {
        CastARay();
        CastARayForAnimals();
        if (lastFrameHit != IsHitting) {
            lastFrameHit = IsHitting;
            // UI manager Added Later
        }
        if (lastFrameHitAnimal != IsHittingAnimal) { 
            lastFrameHitAnimal = IsHittingAnimal;
        }
    }

    private void CastARay()
    {
        // Center of screen (0.5, 0.5)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Use the LayerMask to ignore things like the player's own collider
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistanceInteraction, interactableLayer))
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

    private void CastARayForAnimals()
    {
        // Center of screen (0.5, 0.5)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // Use the LayerMask to ignore things like the player's own collider
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistanceBulletHit, animalLayer))
        {
            HitAnimal = hit;
            IsHittingAnimal = true;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
        else
        {
            HitAnimal = default;
            IsHittingAnimal = false;
        }
    }

    private void OnDrawGizmos()
    {
        
    }

}
