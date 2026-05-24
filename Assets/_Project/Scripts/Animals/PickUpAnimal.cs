using UnityEngine;

public class PickUpAnimal : MonoBehaviour, IInteractableAnimal
{
    [SerializeField] private Animal m_Animal;
    [SerializeField] private AnimalAI m_AnimalAI;
    

    private void Start()
    {
        if(m_Animal == null) m_Animal = GetComponent<Animal>();
        if(m_AnimalAI == null) m_AnimalAI = GetComponent<AnimalAI>();

    }
    public void OnInteract(Transform inventory)
    {
        if (m_Animal.State == AnimalState.InCapacitated || m_Animal.State == AnimalState.Dead) {
            m_AnimalAI.StopIncapacitatedTimer();
            transform.SetParent(inventory);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
