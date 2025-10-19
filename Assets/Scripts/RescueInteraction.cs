using UnityEngine;
using UnityEngine.Audio;

public class RescueInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask animalLayer;
    [SerializeField] private DiverMovement playerDiverMovementReference; 

    private RescueAnimal closerAnimal;
    private RescueAnimal currentAnimal;

    [SerializeField] private AudioSource rescueAudioSource;
    [SerializeField] private AudioClip rescueSound;
    [SerializeField] private float rescueVolume = 1f;


    private void Start()
    {
        if (rescueAudioSource == null)
            rescueAudioSource = GetComponent<AudioSource>();

        if (rescueAudioSource == null)
            rescueAudioSource = gameObject.AddComponent<AudioSource>();
        rescueAudioSource.loop = false;
        rescueAudioSource.playOnAwake = false;
        rescueAudioSource.volume = rescueVolume;
    }



    void Update()
    {
        CheckForAnimal();

        if (closerAnimal != null && !RescueAnimal.hasActiveRescue)
        {
            float timeLimit = closerAnimal.data != null ? closerAnimal.data.rescueTimeLimit : 0f;

            RescueUIManager.Instance.UpdateRescuePrompt(closerAnimal.transform.position, timeLimit);

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlaySound(rescueSound);
                closerAnimal.Rescue(playerDiverMovementReference);
                
                RescueUIManager.Instance.HideRescuePrompt();
            }
        }
        else
        {
            RescueUIManager.Instance.HideRescuePrompt();
        }
    }

    void CheckForAnimal()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, animalLayer);

        closerAnimal = hits.Length > 0
            ? hits[0].GetComponent<RescueAnimal>()
            : null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    public RescueAnimal GetCurrentAnimal()
    {
        return currentAnimal;
    }

    public void SetCurrentAnimal(RescueAnimal animal)
    {
        currentAnimal = animal;
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && rescueAudioSource != null)
        {
            rescueAudioSource.PlayOneShot(clip);
            //Debug.Log($"ShopSystem - Sonido reproducido: {clip.name}");
        }
    }
}
