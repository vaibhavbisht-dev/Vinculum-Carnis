using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeigerTargetAnimals : MonoBehaviour
{
    public static GeigerTargetAnimals Instance { get; private set; }



    [Header("Animal Settings")]
    [SerializeField] private GameObject _animalPrefab;
    [SerializeField] private List<Transform> _targetAnimals = new List<Transform>();
    [SerializeField] private Transform[] _spawnLocations;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform[] _travelLocations;
    [SerializeField] private int _maxAnimalCount = 5;

    [Header("Performance Settings")]
    [Tooltip("How often the Geiger counter updates its target (in seconds).")]
    [SerializeField] private float _targetCheckInterval = 0.2f; // Runs 5 times a second
    [SerializeField] private bool _trackingEnabled = true;


    private List<Transform> _sacrificedAnimals = new List<Transform>();

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
        // Start the repeating check when the script initializes
        StartCoroutine(TargetCheckRoutine());
        SpawnAnimals();
    }

    // This replaces the Update() method
    private IEnumerator TargetCheckRoutine()
    {
        while (_trackingEnabled)
        {
            UpdateTargetAnimalCounter();
            // Pauses the loop for the specified time before checking again
            yield return new WaitForSeconds(_targetCheckInterval);
        }
    }

    private void SpawnAnimals()
    {
        // Safety check to prevent errors if no spawn points are assigned
        if (_spawnLocations == null || _spawnLocations.Length == 0)
        {
            Debug.LogWarning("Spawn locations array is empty or null!");
            return;
        }

        int lastSpawnIndex = -1;

        // Scenario 1: Initial spawn (Instantiate new animals)
        if (_targetAnimals.Count == 0 && _sacrificedAnimals.Count == 0)
        {
            for (int i = 0; i < _maxAnimalCount; i++)
            {
                Transform spawnPoint = GetNextSpawnPoint(ref lastSpawnIndex);

                if (spawnPoint != null)
                {
                    GameObject newAnimal = Instantiate(_animalPrefab, spawnPoint.position, spawnPoint.rotation);
                    _targetAnimals.Add(newAnimal.transform);

                    if (newAnimal.TryGetComponent(out Animal animalComponent))
                    {
                        animalComponent.Init();
                    }
                }
            }
        }
        // Scenario 2: Respawn sacrificed animals
        else if (_targetAnimals.Count == 0 && _sacrificedAnimals.Count > 0)
        {
            // FIX: Copy the elements, don't just share the reference
            _targetAnimals.AddRange(_sacrificedAnimals);
            _sacrificedAnimals.Clear();

            for (int i = 0; i < _targetAnimals.Count; i++)
            {
                Transform animal = _targetAnimals[i];
                Transform spawnPoint = GetNextSpawnPoint(ref lastSpawnIndex);

                if (spawnPoint != null)
                {
                    animal.position = spawnPoint.position;
                    animal.rotation = spawnPoint.rotation;
                }

                if (animal.TryGetComponent(out Animal animalComponent))
                {
                    animalComponent.Init();
                }
            }
        }

    }

    private Transform GetNextSpawnPoint(ref int lastIndex)
    {
        int spawnIndex = VBHelpers.GetRandomValuefromArrayLength(_spawnLocations.Length);

        // Ensure a different spawn point from the last one
        if (spawnIndex == lastIndex)
        {
            spawnIndex = (spawnIndex + 1) % _spawnLocations.Length;
        }

        // FIX: Update the tracking variable for the next loop iteration
        lastIndex = spawnIndex;

        return _spawnLocations[spawnIndex];
    }

    public Vector3 GetRandomTravelLocation()
    {
        // FIX: Check for null or empty BEFORE accessing length
        if (_travelLocations == null || _travelLocations.Length == 0)
        {
            return Vector3.positiveInfinity;
        }

        int index = VBHelpers.GetRandomValuefromArrayLength(_travelLocations.Length);

        // FIX: Safely check the specific element
        if (_travelLocations[index] == null)
        {
            return Vector3.positiveInfinity;
        }

        return _travelLocations[index].position;
    }


    private void UpdateTargetAnimalCounter()
    {
        Transform animal = GetNearestAnimal(_player);

        if (animal == null || GeigerCounterManager.Instance.CurrentRadiationSource() == animal) return;

        GeigerCounterManager.Instance.SetRadiationSource(animal);
    }

    private Transform GetNearestAnimal(Transform playerTransform)
    {
        if (_targetAnimals == null || _targetAnimals.Count == 0) return null;

        Transform nearestAnimal = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = 0; i < _targetAnimals.Count; i++)
        {
            Transform animal = _targetAnimals[i];

            if (animal == null) continue;

            float sqrDistanceToAnimal = (animal.position - playerTransform.position).sqrMagnitude;

            if (sqrDistanceToAnimal < nearestDistanceSqr)
            {
                nearestDistanceSqr = sqrDistanceToAnimal;
                nearestAnimal = animal;
            }
        }

        return nearestAnimal;
    }

    public void OnAnimalSacrificed(Transform animal)
    {
        _targetAnimals.Remove(animal);
        _sacrificedAnimals.Add(animal);
        if (_targetAnimals.Count == 0)
        {
            SpawnAnimals();
        }

    }

}