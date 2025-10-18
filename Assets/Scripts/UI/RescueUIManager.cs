using UnityEngine;
using TMPro;

public class RescueUIManager : MonoBehaviour
{
    public static RescueUIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI timerText;

    private void Awake()
    {
        // Singleton simple para acceso global
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (interactText) interactText.gameObject.SetActive(false);
        if (timerText) timerText.gameObject.SetActive(false);
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
}
