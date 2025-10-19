using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUI : MonoBehaviour
{
    [Header("OxygenBar")]
    [SerializeField] private OxygenSystem oxygenSystem;

    [SerializeField] private Image barFill;                
    [SerializeField] private TextMeshProUGUI percentText; 

    [Header("Visual")]
    [SerializeField, Range(1f, 20f)] private float smooth = 8f;
    [SerializeField] private Gradient colorByPercent; 
    [SerializeField] private Color fallbackLow = new Color(0.9f, 0.2f, 0.2f);
    [SerializeField] private Color fallbackHigh = new Color(0.2f, 0.8f, 0.6f);

    private float currentFill = 1f;

    void Reset()
    {
        if (oxygenSystem == null) oxygenSystem = FindFirstObjectByType<OxygenSystem>();
        if (barFill == null) barFill = GetComponentInChildren<Image>();
        if (percentText == null) percentText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (oxygenSystem == null || barFill == null) return;

        float t = Mathf.Clamp01(oxygenSystem.GetOxygenPercent());

        currentFill = Mathf.Lerp(currentFill, t, Time.deltaTime * smooth);
        barFill.fillAmount = currentFill;

        if (percentText != null)
        {
            int pct = Mathf.RoundToInt(t * 100f);
            percentText.text = pct + "%";
        }

        if (colorByPercent != null)
        {
            barFill.color = colorByPercent.Evaluate(t);
        }
        else
        {
            barFill.color = Color.Lerp(fallbackLow, fallbackHigh, t);
        }
    }
}
