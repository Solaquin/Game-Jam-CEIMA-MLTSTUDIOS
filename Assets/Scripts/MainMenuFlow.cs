using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // opcional si usas botones
using TMPro;            // opcional si usas TMP

public class MainMenuFlow : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject menuCanvas;      // activo al inicio
    public GameObject contextCanvas;   // desactivado al inicio
    public GameObject creditsCanvas;   // desactivado al inicio

    [Header("Escena del juego")]
    public string gameSceneName = "Game";

    [Header("Contexto")]
    public float contextDuration = 3f;         // segundos que se muestra el contexto
    public bool allowSkipWithAnyKey = false;   // permitir saltar con cualquier tecla
    public Button contextSkipButton;           // opcional: botón "Continuar/Saltar" en contexto

    [Header("Fade (opcional)")]
    public CanvasGroup fade;        // Panel negro con CanvasGroup a full screen (opcional)
    public float fadeDuration = 0.25f;

    private bool _skipping = false;
    private Coroutine _running;

    void Start()
    {
        // Estado inicial de los canvas
        SetActiveSafe(menuCanvas, true);
        SetActiveSafe(contextCanvas, false);
        SetActiveSafe(creditsCanvas, false);

        // Fade desde negro (opcional)
        if (fade) { fade.alpha = 1f; StartCoroutine(FadeTo(0f)); }

        // Botón saltar del contexto (opcional)
        if (contextSkipButton)
        {
            contextSkipButton.onClick.RemoveAllListeners();
            contextSkipButton.onClick.AddListener(SkipContext);
        }
    }

    void Update()
    {
        // Saltar contexto con cualquier tecla (opcional)
        if (allowSkipWithAnyKey && contextCanvas && contextCanvas.activeSelf && !_skipping)
        {
            if (Input.anyKeyDown) SkipContext();
        }
    }

    // ======= Botones del Menú =======

    // Botón "Jugar"
    public void OnPlayClicked()
    {
        SwitchTo(contextCanvas);
        _skipping = false;
        StopRunning();
        _running = StartCoroutine(ContextThenLoad());
    }

    // Botón "Créditos"
    public void OnCreditsClicked()
    {
        SwitchTo(creditsCanvas);
    }

    // Botón "Volver" (desde Créditos o Contexto)
    public void OnBackToMenu()
    {
        _skipping = true; // por si venimos del contexto
        StopRunning();
        SwitchTo(menuCanvas);
    }

    // ======= Botones dentro del Contexto (opcional) =======

    // Botón "Continuar/Saltar" en Contexto
    public void SkipContext()
    {
        if (_skipping) return;
        _skipping = true;
        StopRunning();
        _running = StartCoroutine(LoadGame());
    }

    // ======= Flujo interno =======

    private IEnumerator ContextThenLoad()
    {
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
        yield return FadeTo(1f); // fade out
        var op = SceneManager.LoadSceneAsync(gameSceneName);
        while (!op.isDone) yield return null;
    }

    private void SwitchTo(GameObject target)
    {
        SetActiveSafe(menuCanvas, target == menuCanvas);
        SetActiveSafe(contextCanvas, target == contextCanvas);
        SetActiveSafe(creditsCanvas, target == creditsCanvas);
    }

    private void SetActiveSafe(GameObject go, bool active)
    {
        if (go && go.activeSelf != active) go.SetActive(active);
    }

    private void StopRunning()
    {
        if (_running != null) StopCoroutine(_running);
        _running = null;
    }

    // ======= Fade helper (opcional) =======
    private IEnumerator FadeTo(float target)
    {
        if (!fade || fadeDuration <= 0f) yield break;

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
