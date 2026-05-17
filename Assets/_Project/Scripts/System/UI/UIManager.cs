using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Geiger Counter Settings")]
    [SerializeField] private GameObject geigerCounterUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ShowGeigerCounter(bool value)
    {
        geigerCounterUI.SetActive(value);
    }





}