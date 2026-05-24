using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animal))]
public class AnimalAI : MonoBehaviour
{
    [SerializeField] private Animal m_Animal;
    

    [Header("Timers & Speeds")]
    //[SerializeField] private float bleedingTime = 5f;
    [SerializeField] private float incapacitatedTime = 10f;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float bleedingSpeed = 5f;
    [SerializeField] private bool isIncapacitatedTimerActive = true;

    [Header("Pathfinding Adjustments")]
    [Tooltip("How far around the target location the animal can wander to prevent stacking.")]
    [SerializeField] private float destinationSpreadRadius = 3f;

    // Internal tracking
    [SerializeField] private NavMeshAgent agent;
    private float stateTimer = 0f;
    private bool isWaiting = false;

    private AnimalState previousState;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (m_Animal == null) m_Animal = GetComponent<Animal>();

        // Ensure the agent has a small stopping distance so it doesn't force exact pixel-perfect arrivals
        if (agent.stoppingDistance < 0.1f) agent.stoppingDistance = 0.5f;
    }

    private void Start()
    {
        previousState = m_Animal.State;
        ApplyStateChangeEffects(previousState);
    }

    private void Update()
    {
        if (m_Animal.State != previousState)
        {
            ApplyStateChangeEffects(m_Animal.State);
            previousState = m_Animal.State;
        }

        UpdateState();
    }

    private void ApplyStateChangeEffects(AnimalState newState)
    {
        StopAllCoroutines();
        stateTimer = 0f;
        isWaiting = false;

        switch (newState)
        {
            case AnimalState.Alive:
                // Added safety check
                if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.speed = speed;
                    agent.isStopped = false;
                }
                SetNewDestination(); // Note: SetNewDestination already has its own safety check inside it!
                break;

            case AnimalState.Bleeding:
                // Added safety check
                if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.speed = bleedingSpeed;
                    agent.isStopped = false;
                }
                SetNewDestination();
                break;

            case AnimalState.InCapacitated:
                // Added safety check (This is what caused Line 76 to throw the error)
                if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }
                break;

            case AnimalState.OnSacrifice:
            case AnimalState.Dead:
                // You already had the safety check here!
                if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                break;
        }
    }

    private void UpdateState()
    {
        switch (m_Animal.State)
        {
            case AnimalState.Alive:
                OnAlive();
                break;
            case AnimalState.Bleeding:
                OnBleeding();
                break;
            case AnimalState.InCapacitated:
                OnIncapacitated();
                break;
            case AnimalState.OnSacrifice:
                OnSacrifice();
                break;
        }
    }

    private void OnAlive()
    {
        // FIX: Check if we are within the stopping distance, not less than zero
        if (!isWaiting && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                StartCoroutine(WaitTimerToFetchNewDestination(2f));
            }
        }
    }

    private void OnBleeding()
    {
        if (!isWaiting && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                StartCoroutine(WaitTimerToFetchNewDestination(0.5f));
            }
        }
    }

    private void OnIncapacitated()
    {
        if(!isIncapacitatedTimerActive) return;
        stateTimer += Time.deltaTime;
        if (stateTimer >= incapacitatedTime)
        {
            m_Animal.ChangeState(AnimalState.Dead);
        }
    }
    public void StartIncapacitatedTimer() { 
        isIncapacitatedTimerActive = true;
    }

    public void StopIncapacitatedTimer() {
        isIncapacitatedTimerActive = false;
    }
    private void OnSacrifice()
    {
        // Logic for when the player picks up the animal/harvests it
        
    }

    private IEnumerator WaitTimerToFetchNewDestination(float waitTime)
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        SetNewDestination();
        isWaiting = false;
    }

    private void SetNewDestination()
    {
        Vector3 baseTarget = GeigerTargetAnimals.Instance.GetRandomTravelLocation();

        if (baseTarget == Vector3.positiveInfinity)
        {
            if (!isWaiting) StartCoroutine(WaitTimerToFetchNewDestination(2f));
            return;
        }

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            // FIX: Add a random offset to the target so animals don't bunch up on the exact same coordinate
            Vector3 randomDirection = Random.insideUnitSphere * destinationSpreadRadius;
            randomDirection += baseTarget;

            // Ensure the new offset point is actually on the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, destinationSpreadRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                // Fallback to exact location if the random point is off the edge of the map
                agent.SetDestination(baseTarget);
            }
        }
    }

}
