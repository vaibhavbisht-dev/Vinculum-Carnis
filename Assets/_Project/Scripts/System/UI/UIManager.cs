using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform player;

    [Header("Geiger Counter Settings")]
    [SerializeField] private GameObject geigerCounterUI;
    [SerializeField] private RectTransform geigerArrow;
    [SerializeField] private bool isGeigerActive = false;

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
        isGeigerActive = value;
    }



    private void Update()
    {
        RotateGeigerArrow();
    }


    private void RotateGeigerArrow() {
        if (!isGeigerActive) return;
        if (GeigerCounterManager.Instance.CurrentRadiationSource() == null) {
            geigerArrow.gameObject.SetActive(false);
            return;
        }
        geigerArrow.gameObject.SetActive(true);
        float arrowAngle = VBHelpers.GetArrowRotationAngle(player, GeigerCounterManager.Instance.CurrentRadiationSource().position);
        geigerArrow.eulerAngles = new Vector3(0, 0, -arrowAngle);
    }





}