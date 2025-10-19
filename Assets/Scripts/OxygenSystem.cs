using UnityEngine;

public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 100;
    [SerializeField] private float oxygenDrainMoving = 2f;
    [SerializeField] private float oxygenDrainIdle = 1f;

    [Header("Other References")]
    [SerializeField] GameObject noOxygenCanvas;
    [SerializeField] private SurfaceZone surfaceZone;

    private float currentOxygen;
    private bool isInSafeZone = false;
    private Rigidbody rb;
    private DiverMovement diverMovement;
    private RescueInteraction rescueInteraction;
    private VacuumSystem vacuumSystem;
    private BagSystem bagSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        diverMovement = GetComponent<DiverMovement>();
        bagSystem = GetComponent<BagSystem>();
        rescueInteraction = GetComponent<RescueInteraction>();
        vacuumSystem = GetComponent<VacuumSystem>();
        currentOxygen = maxOxygen;
        noOxygenCanvas.SetActive(false);
    }

    void Update()
    {
        HandleOxygen();
        //Debug.Log($"Oxygen: {currentOxygen}");
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
            DeadLogic();
        }
    }

    public void DeadLogic()
    {
        diverMovement.EnableMovement(false);
        bagSystem.ClearBag();
        noOxygenCanvas.SetActive(true);
        vacuumSystem.enabled = false;
        if (RescueAnimal.hasActiveRescue && rescueInteraction.GetCurrentAnimal() != null)
        {
            rescueInteraction.GetCurrentAnimal().RescueFailed();
        }
        rescueInteraction.enabled = false;
    }

    public void RestartDiver()
    {
        surfaceZone.GoToBase();
        noOxygenCanvas.SetActive(false);
        rescueInteraction.enabled = true;
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

    public void SetMaxOxygen(float newAmount)
    {
        maxOxygen = newAmount;
        Debug.Log($"Nuevo oxígeno máximo: {maxOxygen}");
    }

    public float GetMaxOxygen()
    {
        return maxOxygen;
    }
}
