using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;           // opcional, solo si usas TextMeshPro
using UnityEngine.UI; // opcional, solo si usas Button/Slider

public class MainMenuFlow : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject menuCanvas;    // Canvas del menú principal (con botón Jugar)
    public GameObject contextCanvas; // Canvas del contexto (déjalo desactivado al inicio)

    [Header("Escena del juego")]
    public string gameSceneName = "Game"; // Nombre exacto de la escena de juego (en Build Settings)

    [Header("Contexto")]
    public float contextDuration = 3f;    // Segundos que se muestra el contexto antes de cargar la escena
    public bool allowSkipWithAnyKey = false; // Permitir saltar el contexto con cualquier tecla
    public Button skipButton;             // Opcional: botón "Continuar"/"Saltar" en el contexto

    [Header("Fade (opcional)")]
    public CanvasGroup fade;       // Un CanvasGroup negro a pantalla completa (opcional)
    public float fadeDuration = 0.25f;

    private bool _skipping = false;

    void Start()
    {
        // Estado inicial
        if (menuCanvas) menuCanvas.SetActive(true);
        if (contextCanvas) contextCanvas.SetActive(false);

        // Fade desde negro (opcional)
        if (fade) { fade.alpha = 1f; StartCoroutine(FadeTo(0f)); }

        // Botón de saltar (opcional)
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipContext);
        }
    }

    void Update()
    {
        // Permitir saltar con cualquier tecla (opcional)
        if (allowSkipWithAnyKey && _skipping == false && contextCanvas != null && contextCanvas.activeSelf)
        {
            if (Input.anyKeyDown) SkipContext();
        }
    }

    // Llama este método desde el botón "Jugar" del menú
    public void OnPlayClicked()
    {
        if (menuCanvas) menuCanvas.SetActive(false);
        if (contextCanvas) contextCanvas.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(ContextThenLoad());
    }

    // Botón "Continuar/Saltar" del contexto (opcional)
    public void SkipContext()
    {
        if (_skipping) return;
        _skipping = true;
        StopAllCoroutines();
        StartCoroutine(LoadGame());
    }

    // ------ Flujo: mostrar contexto -> esperar -> cargar juego ------
    private IEnumerator ContextThenLoad()
    {
        _skipping = false;

        // (Si quieres un fade-in del contexto, descomenta)
        // yield return FadeTo(0f);

        float t = 0f;
        while (t < contextDuration && !_skipping)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!_skipping)
            yield return LoadGame();
    }

    private IEnumerator LoadGame()
    {
        // Fade out (opcional)
        yield return FadeTo(1f);

        var op = SceneManager.LoadSceneAsync(gameSceneName);
        while (!op.isDone) yield return null;
    }

    // ------ Helper de Fade (opcional) ------
    private IEnumerator FadeTo(float target)
    {
        if (fade == null || fadeDuration <= 0f)
            yield break;

        float start = fade.alpha;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            fade.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }
        fade.alpha = target;
    }
}
