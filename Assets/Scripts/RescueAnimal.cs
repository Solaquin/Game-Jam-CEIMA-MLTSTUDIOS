using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class RescueAnimal : MonoBehaviour
{
    [Header("Animal Data")]
    public AnimalData data;

    private bool isRescued = false;
    private bool timerActive = false;
    private float timer;

    public static bool hasActiveRescue = false; // Evita más de un rescate a la vez

    private Renderer meshRenderer;
    private Collider animalCollider;

    [SerializeField] private AudioSource rescueAudioSource;
    [SerializeField] private AudioClip rescueStartTimer;
    [SerializeField] private AudioClip failedRescue;
    [SerializeField] private AudioClip finishRescue;
    [SerializeField] private float TimerVolume = 0.8f;
    [SerializeField] private float FailedRescueVolume = 0.8f;
    [SerializeField] private float finishVolume = 0.8f;

    [Header("Reward UI (TMP en tu Canvas)")]
    [SerializeField] private TextMeshProUGUI rewardText;   
    [SerializeField] private float rewardShowSeconds = 1.5f; 
    private Coroutine rewardRoutine;

    private void Start()
    {
        if (data != null)
            timer = data.rescueTimeLimit;

        if (rescueAudioSource == null)
        {
            rescueAudioSource = GetComponent<AudioSource>();
        }
        if (rescueAudioSource == null)
        {
            rescueAudioSource = gameObject.AddComponent<AudioSource>();
        }
         

        rescueAudioSource.loop = false;
        rescueAudioSource.playOnAwake = false;

        meshRenderer = GetComponentInChildren<Renderer>();
        animalCollider = GetComponent<Collider>();
        if (rewardText != null) rewardText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (timerActive)
        {
            timer -= Time.deltaTime;
            RescueUIManager.Instance.UpdateTimer(timer); 

            if (timer <= 0)
            {
                timerActive = false;
                RescueUIManager.Instance.HideTimer();
                RescueFailed();
            }
        }
    }

    public void Rescue(DiverMovement diver)
    {
        if (isRescued || hasActiveRescue)
        {
            Debug.Log("Ya estás rescatando a un animal o este ya fue rescatado.");
            return;
        }

        isRescued = true;
        timerActive = true;
        hasActiveRescue = true;

        // Reproducir sonido de inicio de rescate
        PlaySound(rescueStartTimer);
        Debug.Log($"{data.animalName} rescatado. Tiempo: {timer:F1}s para volver a base.");

        // En lugar de SetActive(false), solo ocultamos el modelo
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (animalCollider != null) animalCollider.enabled = false;

        RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
        if (rescueInteraction != null)
        {
            rescueInteraction.SetCurrentAnimal(this);
        }
    }

    public void ReachBase()
    {
        if (!isRescued) return;

        timerActive = false;
        RescueUIManager.Instance.HideTimer();   

        if (timer > 0)
        {
            PauseSound(rescueStartTimer);
            
            Debug.Log($"{data.animalName} llegó a salvo. +{data.rewardMoney} monedas!");
            var diver = FindFirstObjectByType<DiverMovement>();
            GiveReward(diver);
            RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
            if (diver != null)
            {
                rescueInteraction.SetCurrentAnimal(null);
            }
        }
        else
        {
            PlaySound(failedRescue);
            PauseSound(rescueStartTimer);
            Debug.Log($"{data.animalName} no sobrevivió al rescate :(");
        }

        hasActiveRescue = false;
        if (rewardRoutine != null) StopCoroutine(rewardRoutine);
        {
            StartCoroutine(DestroyAfterReward());
        }
        
    }

    private void GiveReward(DiverMovement diver)
    {
        PlaySound(finishRescue);
        PauseSound(rescueStartTimer);
        PlayerStats playerStats = diver.GetComponent<PlayerStats>();
        
        
        if (playerStats != null)
        {
            playerStats.money += data.rewardMoney;
        }
        ShowRewardMessage();
    }

    public void RescueFailed()
    {
        Debug.Log($"{data.animalName} no logró sobrevivir al rescate (tiempo agotado).");
        PlaySound(failedRescue);
        PauseSound(rescueStartTimer);
        hasActiveRescue = false;
        isRescued = false;
        timerActive = false;
        meshRenderer.enabled = true;
        animalCollider.enabled = true;
        timer = data.rescueTimeLimit;
        RescueUIManager.Instance.HideTimer();

        var diver = FindFirstObjectByType<DiverMovement>();
        RescueInteraction rescueInteraction = diver.GetComponent<RescueInteraction>();
        if (diver != null)
        {
            rescueInteraction.SetCurrentAnimal(null);
        }
        if (rewardText != null) rewardText.gameObject.SetActive(false);
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && rescueAudioSource != null)
        {
            rescueAudioSource.PlayOneShot(clip);
            //Debug.Log($"ShopSystem - Sonido reproducido: {clip.name}");
        }
    }
    private void PauseSound(AudioClip clip)
    {
        if (rescueAudioSource != null && rescueAudioSource.isPlaying)
        {
            rescueAudioSource.Pause();

        }
}
    private void ShowRewardMessage()
    {
        if (rewardText == null) return;

        rewardText.text = $"¡{data.animalName} salvado!  +{data.rewardMoney} monedas";
        rewardText.gameObject.SetActive(true);

        if (rewardRoutine != null) StopCoroutine(rewardRoutine);
        rewardRoutine = StartCoroutine(HideRewardAfterDelay());
    }

    private System.Collections.IEnumerator HideRewardAfterDelay()
    {
        float t = 0f;
        while (t < rewardShowSeconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if (rewardText != null) rewardText.gameObject.SetActive(false);
    }
    private System.Collections.IEnumerator DestroyAfterReward()
    {
        // Espera exactamente lo que dura el mensaje en pantalla
        float t = 0f;
        while (t < rewardShowSeconds)
        {
            t += Time.unscaledDeltaTime;   // para que no dependa del timescale
            yield return null;
        }

        // Asegura que el texto quede oculto
        if (rewardText != null) rewardText.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}

