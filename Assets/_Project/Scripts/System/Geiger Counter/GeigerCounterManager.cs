using UnityEngine;
using UnityEngine.InputSystem;

public class GeigerCounterManager : MonoBehaviour
{
    // Singleton instance remains public so other scripts can access the manager
    public static GeigerCounterManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform radiationSource;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private InputActionReference toggleGeigerCounter;

    [Header("Geiger Settings")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float minDelay = 0.05f;    // Fastest clicking (close to source)
    [SerializeField] private float maxDelay = 1.5f;     // Slowest clicking (far from source)

    [Tooltip("Controls the tension. 0 is at the source, 1 is at maxDistance. Bow the line downward to make it tick faster earlier.")]
    [SerializeField] private AnimationCurve responseCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // Internal state
    private float nextClickTime;

    // Public getter, private setter: Other scripts can read this, but only this script can change it.
    public bool IsActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        IsActive = !IsActive;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGeigerCounter(IsActive);
        }

        toggleGeigerCounter.action.Enable();
    }

    private void OnDestroy()
    {
        toggleGeigerCounter.action.Disable();
    }

    void Update()
    {
        ToggleGeigerCounter();
        GeigerCounter();
    }

    private void ToggleGeigerCounter()
    {
        if (toggleGeigerCounter.action.WasPressedThisFrame())
        {
            IsActive = !IsActive;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGeigerCounter(IsActive);
            }
        }
    }

    private void GeigerCounter()
    {
        if (!IsActive) return;
        if (radiationSource == null) return;

        // 1. Calculate distance
        float distance = Vector3.Distance(transform.position, radiationSource.position);

        // 2. Only click if within range
        if (distance < maxDistance)
        {
            if (Time.time >= nextClickTime)
            {
                // 3. Play the audio clip
                clickSound.PlayOneShot(clickSound.clip);

                // 4. Calculate raw percentage (0 is at source, 1 is at maxDistance)
                float rawPercentage = distance / maxDistance;

                // 5. Evaluate the curve to get a non-linear response
                float curvedPercentage = responseCurve.Evaluate(rawPercentage);

                // 6. Calculate next click time based on the CURVED percentage
                float baseDelay = Mathf.Lerp(minDelay, maxDelay, curvedPercentage);

                // 7. Add random variation for realism
                float randomNoise = Random.Range(-0.02f, 0.02f);
                nextClickTime = Time.time + Mathf.Max(0.01f, baseDelay + randomNoise);
            }
        }
    }

    /// <summary>
    /// Use this method if another script needs to update the radiation source dynamically at runtime.
    /// </summary>
    public void SetRadiationSource(Transform newSource)
    {
        radiationSource = newSource;
    }
}
