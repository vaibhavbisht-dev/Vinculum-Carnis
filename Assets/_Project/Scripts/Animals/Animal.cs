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

    public float BloodValue
    {
        get
        {
            if (state == AnimalState.Dead)
            {
                return deadBloodValue;
            }
            return defaultBloodValue;
        }
    }

    public AnimalState State => state;

    // We no longer need Awake() to fetch AnimalAI!

    public void Init()
    {
        ChangeState(AnimalState.Alive);
    }

    public void ChangeState(AnimalState newState)
    {
        if (state == newState) return; // Prevent double-calling the same state

        state = newState;

        // Removed animalAI.UpdateStateAfterStateChange();
        // AnimalAI now instantly detects this change in its own Update() loop.
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            ChangeState(AnimalState.Dead);
        }
        else if (health < 60 && state != AnimalState.Bleeding)
        {
            ChangeState(AnimalState.Bleeding);
        }else if(health < 30 && state != AnimalState.InCapacitated)
        {
            ChangeState(AnimalState.InCapacitated);
        }
    }
}

public enum AnimalState { 
    Alive,
    Bleeding,
    InCapacitated,
    OnSacrifice,
    Dead
}
