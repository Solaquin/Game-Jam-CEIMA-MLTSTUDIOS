using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;       
using TMPro;

public class SceneManagerSimple : MonoBehaviour
{
    public static SceneManagerSimple I { get; private set; }

    [Header("Pantalla de carga (opcional)")]
    [Tooltip("Canvas con un Slider y/o un TMP_Text. Déjalo null si no quieres pantalla de carga.")]
    public GameObject loadingCanvas;
    public Slider progressBar;              
    public TextMeshProUGUI progressText;    


    [Header("Fade (opcional)")]
    public CanvasGroup fadeGroup;         
    public float fadeDuration = 0.25f;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Asegura estado inicial UI opcional
        if (loadingCanvas) loadingCanvas.SetActive(false);
        if (fadeGroup) fadeGroup.alpha = 0f;
        
    }
    public void LoadByName(string sceneName) => StartCoroutine(LoadRoutine(() => SceneManager.LoadSceneAsync(sceneName)));
    public void LoadByIndex(int buildIndex) => StartCoroutine(LoadRoutine(() => SceneManager.LoadSceneAsync(buildIndex)));
    public void Reload() => LoadByIndex(SceneManager.GetActiveScene().buildIndex);
    public void Next() => LoadByIndex((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    public void Previous()
    {
        int idx = SceneManager.GetActiveScene().buildIndex - 1;
        if (idx < 0) idx = SceneManager.sceneCountInBuildSettings - 1;
        LoadByIndex(idx);
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadRoutine(System.Func<AsyncOperation> loadOpFactory)
    {
        yield return Fade(1f);

        if (loadingCanvas) loadingCanvas.SetActive(true);
        UpdateProgress(0f);

        AsyncOperation op = loadOpFactory();
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            UpdateProgress(op.progress);
            yield return null;
        }

        UpdateProgress(1f);

        yield return null;

        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        if (loadingCanvas) loadingCanvas.SetActive(false);
        yield return Fade(0f);
    }

    private void UpdateProgress(float t)
    {
        float normalized = Mathf.InverseLerp(0f, 0.9f, Mathf.Clamp01(t));
        if (progressBar) progressBar.value = normalized;
        if (progressText) progressText.text = Mathf.RoundToInt(normalized * 100f) + "%";
    }

    private IEnumerator Fade(float target)
    {
        if (!fadeGroup || fadeDuration <= 0f) yield break;

        float start = fadeGroup.alpha;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = target;
    }
}
