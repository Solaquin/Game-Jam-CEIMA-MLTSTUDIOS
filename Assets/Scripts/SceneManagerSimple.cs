using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;        // opcional (si usas Button/Slider)
using TMPro;                 // opcional (si usas TMP)

public class SceneManagerSimple : MonoBehaviour
{
    public static SceneManagerSimple I { get; private set; }

    [Header("Escena del juego")]
    [Tooltip("Nombre de la escena jugable que quieres cargar después del contexto.")]
    public string firstSceneName = "Main";

    [Header("Intro / Contexto")]
    [Tooltip("Panel/Canvas con tu texto de contexto y botón 'Continuar'.")]
    public GameObject introCanvas;
    [Tooltip("Opcional: Título de la intro.")]
    public TextMeshProUGUI introTitle;
    [Tooltip("Opcional: Cuerpo/Descripción de la intro.")]
    public TextMeshProUGUI introBody;
    [Tooltip("Si está activo: cualquier tecla continúa (además del botón).")]
    public bool anyKeyToContinue = true;

    [Header("Cuándo mostrar la intro")]
    [Tooltip("Si true, la intro aparece al iniciar la escena. Si false, la mostrarás manualmente (ej. desde el botón Jugar).")]
    public bool showIntroOnStart = false;

    [Header("Fade (opcional)")]
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.25f;

    // Para mostrar la intro solo una vez por ejecución si usas showIntroOnStart
    private static bool s_introShownOnce = false;
    private bool _waitingIntroInput = false;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if (fadeGroup) fadeGroup.alpha = 1f; // arrancamos en negro → haremos fade-in
    }



    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;

        // Solo mostrar intro automáticamente si así lo configuraste
        if (showIntroOnStart && !s_introShownOnce && introCanvas != null)
        {
            introCanvas.SetActive(true);
            StartCoroutine(Fade(0f)); // fade in desde negro
            s_introShownOnce = true;
            _waitingIntroInput = true;
        }
        else
        {
            // Si no vas a mostrar intro al inicio, simplemente quita el fade negro
            StartCoroutine(Fade(0f));
        }
    }

    void Update()
    {
        if (_waitingIntroInput && anyKeyToContinue && Input.anyKeyDown)
        {
            OnIntroContinue();
        }
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Button extBtn = GameObject.Find("ExitBtn")?.GetComponent<Button>();
        if (extBtn != null)
        {
            extBtn.onClick.RemoveAllListeners();
            extBtn.onClick.AddListener(() => SceneManagerSimple.I.LoadByName("MainMenu"));
        }
    }

    // ---------- BOTÓN JUGAR (desde tu Main Menu) ----------
    public void OnPlayClicked()
    {
        // Mostrar el panel de contexto y esperar input
        if (introCanvas != null)
        {
            introCanvas.SetActive(true);
            _waitingIntroInput = anyKeyToContinue;
        }
        else
        {
            // Si no tienes panel de contexto, entra directo al juego
            LoadFirstScene();
        }
    }

    // ---------- BOTÓN CONTINUAR (dentro del panel de contexto) ----------
    public void OnIntroContinue()
    {
        if (introCanvas) introCanvas.SetActive(false);
        _waitingIntroInput = false;
        LoadFirstScene();
    }

    // ==============================
    // Carga de primera escena (juego)
    // ==============================
    private void LoadFirstScene()
    {
        if (string.IsNullOrEmpty(firstSceneName))
        {
            Debug.LogWarning("SceneManagerSimple: 'firstSceneName' no asignado.");
            return;
        }

        StartCoroutine(LoadDirect(firstSceneName));
    }

    // Carga sin pantalla de carga; solo con fade (opcional)
    private IEnumerator LoadDirect(string sceneName)
    {
        yield return Fade(1f); // fade out
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        yield return Fade(0f); // fade in
    }

    // ==============================
    // Públicos extra (por si los quieres en otros botones)
    // ==============================
    public void LoadByName(string sceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadDirect(sceneName)); 
    }
    public void Reload() => LoadByName(SceneManager.GetActiveScene().name);
    public void Next()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = (current + 1) % SceneManager.sceneCountInBuildSettings;
        StartCoroutine(LoadByIndexRoutine(next));
    }
    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadByIndexRoutine(int buildIndex)
    {
        yield return Fade(1f);
        var op = SceneManager.LoadSceneAsync(buildIndex);
        while (!op.isDone) yield return null;
        yield return Fade(0f);
    }

    // ==============================
    // Fade helper
    // ==============================
    private IEnumerator Fade(float target)
    {
        if (!fadeGroup || fadeDuration <= 0f) yield break;

        float start = fadeGroup.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = target;
    }
}
