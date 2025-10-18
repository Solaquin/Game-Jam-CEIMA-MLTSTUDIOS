using UnityEngine;

public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float oxygenDrainMoving = 10f;
    [SerializeField] private float oxygenDrainIdle = 2f;

    private float currentOxygen;
    private bool isInSafeZone = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentOxygen = maxOxygen;
    }

    void Update()
    {
        HandleOxygen();
        Debug.Log($"Oxygen: {currentOxygen}");
        //Debug.Log($"Magnitude: {rb.linearVelocity.magnitude}");
    }

    void HandleOxygen()
    {
        if (isInSafeZone)
            return;

        bool isMoving = rb != null && rb.linearVelocity.magnitude > 0.1f;

        float drainRate = isMoving ? oxygenDrainMoving : oxygenDrainIdle;
        currentOxygen -= drainRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        if (currentOxygen <= 0)
        {
            Debug.Log("Sin oxígeno");
        }
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
    }

    public void SetSafeZone(bool value)
    {
        isInSafeZone = value;
    }

    public float GetOxygenPercent()
    {
        return currentOxygen / maxOxygen;
    }
}
