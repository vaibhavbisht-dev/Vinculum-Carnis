using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour, IDamageables
{
    [Header("State")]
    [SerializeField] private AnimalState state = AnimalState.Alive;

    [Header("Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float defaultBloodValue = 50f;
    [SerializeField] private float deadBloodValue = 20f;

    [SerializeField] private LayerMask animalLayerMask;
    [SerializeField] private LayerMask interactableLayerMask;

    // Added reference to the NavMeshAgent
    private NavMeshAgent agent;

    public float BloodValue => state == AnimalState.Dead ? deadBloodValue : defaultBloodValue;
    public AnimalState State => state;

    private void Awake()
    {
        // Cache the agent reference
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Init(); // Ensure the animal sets up properly on spawn
    }

    public void Init()
    {
        ChangeState(AnimalState.Alive);
        health = 100f;
        gameObject.layer = GetLayerFromMask(animalLayerMask);

        if (agent != null) agent.isStopped = false;
    }

    public void ChangeState(AnimalState newState)
    {
        if (state == newState) return;
        state = newState;
    }

    public void TakeDamage(float damageAmount)
    {

        // Prevent taking damage if already dead
        if (state == AnimalState.Dead) return;
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current health: {health - damageAmount}");

        // Clamp health so it never goes below 0
        health = Mathf.Max(0, health - damageAmount);

        if (health <= 0)
        {
            ChangeState(AnimalState.Dead);
            gameObject.layer = GetLayerFromMask(interactableLayerMask);
            StopMovement();
        }
        else if (health < 30 && state != AnimalState.InCapacitated)
        {
            ChangeState(AnimalState.InCapacitated);
            gameObject.layer = GetLayerFromMask(interactableLayerMask);
            StopMovement();
        }
        else if (health < 60 && state != AnimalState.Bleeding)
        {
            ChangeState(AnimalState.Bleeding);
        }
    }

    private void StopMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false; // Completely disables pathfinding
        }
    }

    private int GetLayerFromMask(LayerMask mask)
    {
        if (mask.value == 0) return 0;
        return Mathf.RoundToInt(Mathf.Log(mask.value, 2));
    }
}

public enum AnimalState { 
    Alive,
    Bleeding,
    InCapacitated,
    OnSacrifice,
    Dead
}
