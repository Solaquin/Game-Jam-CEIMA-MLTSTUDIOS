using UnityEngine;
using TMPro;

public class RescueUIManager : MonoBehaviour
{
    public static RescueUIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI rescueTimeText;

    [Header("Position Offsets")]
    public Vector3 interactOffset = new Vector3(0, 2f, 0);
    public Vector3 rescueTimeOffset = new Vector3(0, 1.3f, 0); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (interactText) interactText.gameObject.SetActive(false);
        if (timerText) timerText.gameObject.SetActive(false);
        if (rescueTimeText) rescueTimeText.gameObject.SetActive(false);
    }

    public void ShowInteractText(bool show)
    {
        if (interactText)
            interactText.gameObject.SetActive(show);
    }

    public void UpdateTimer(float time)
    {
        if (!timerText) return;

        if (time > 0)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.CeilToInt(time).ToString();
        }
        else
        {
            timerText.gameObject.SetActive(false);
        }
    }

    public void HideTimer()
    {
        if (timerText)
            timerText.gameObject.SetActive(false);
    }

    public void UpdateRescuePrompt(Vector3 worldPos, float rescueTime)
    {
        if (interactText == null || rescueTimeText == null) return;

        Vector3 interactScreenPos = Camera.main.WorldToScreenPoint(worldPos + interactOffset);
        Vector3 rescueScreenPos = Camera.main.WorldToScreenPoint(worldPos + rescueTimeOffset);

        // Aplicamos las posiciones a los textos
        interactText.transform.position = interactScreenPos;
        rescueTimeText.transform.position = rescueScreenPos;

        // Activamos ambos textos
        interactText.gameObject.SetActive(true);
        rescueTimeText.gameObject.SetActive(true);

        rescueTimeText.text = $"Tiempo: {rescueTime:F0}s";
    }
    public void HideRescuePrompt()
    {
        if (interactText) interactText.gameObject.SetActive(false);
        if (rescueTimeText) rescueTimeText.gameObject.SetActive(false);
    }
}
