using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public enum UIPanelType
{
    Main,
    RescueAnimals,
    GoHome,
    Bag,
    Shop,
    NoOxygen,
    Pause
}

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public struct UIPanelEntry
    {
        public UIPanelType type;
        public GameObject canvas;
    }

    [SerializeField] private List<UIPanelEntry> panelsList;
    public SurfaceZone surfaceZone;
    public GameObject diver;



    private Dictionary<UIPanelType, GameObject> uiPanels;
    private bool isPaused;

    void Awake()
    {
        // Construir el diccionario
        uiPanels = new Dictionary<UIPanelType, GameObject>();
        foreach (var entry in panelsList)
        {
            if (entry.canvas != null)
                uiPanels[entry.type] = entry.canvas;
        }
    }

    void Start()
    {
        // Desactivar todos los paneles
        foreach (var panel in uiPanels.Values)
            panel.SetActive(false);

        // Activar los que quieras al inicio
        uiPanels[UIPanelType.Main].SetActive(true);
        uiPanels[UIPanelType.RescueAnimals].SetActive(true);
        uiPanels[UIPanelType.GoHome].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }

        // Si el juego está pausado, no dejar abrir ninguna otra UI
        if (isPaused)
            return;

        bool noOxygenActive = uiPanels[UIPanelType.NoOxygen].activeSelf;
        bool isAtBase = surfaceZone.IsAtBase();

        // Bloquear todo si el jugador está sin oxígeno
        if (noOxygenActive)
        {
            uiPanels[UIPanelType.Bag].SetActive(false);
            uiPanels[UIPanelType.RescueAnimals].SetActive(false);
            uiPanels[UIPanelType.GoHome].SetActive(false);
            uiPanels[UIPanelType.Shop].SetActive(false);
            return;
        }

        // Control de inventario y tienda
        if (isAtBase)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (uiPanels[UIPanelType.Bag].activeSelf)
                    Toggle(UIPanelType.Bag);

                Toggle(UIPanelType.Shop);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                if (uiPanels[UIPanelType.Shop].activeSelf)
                    Toggle(UIPanelType.Shop);

                Toggle(UIPanelType.Bag);
            }
        }
        else
        {
            // En inmersión: solo inventario
            if (Input.GetKeyDown(KeyCode.I))
                Toggle(UIPanelType.Bag);

            // Cierra tienda si está abierta fuera de la base
            if (uiPanels[UIPanelType.Shop].activeSelf)
                Toggle(UIPanelType.Shop);
        }
    }


    public void Toggle(UIPanelType type)
    {
        if (!uiPanels.ContainsKey(type))
            return;

        bool isActive = uiPanels[type].activeSelf;
        uiPanels[type].SetActive(!isActive);
    }

    public void Show(UIPanelType type)
    {
        if (!uiPanels.ContainsKey(type))
            return;

        foreach (var panel in uiPanels.Values)
            panel.SetActive(false);

        uiPanels[type].SetActive(true);
    }

    public void Hide(UIPanelType type)
    {
        if (!uiPanels.ContainsKey(type))
            return;

        uiPanels[type].SetActive(false);
    }

    public void ToggleInventoryPanel()
    {
        if (uiPanels[UIPanelType.Shop].activeSelf)
            Toggle(UIPanelType.Shop);

        Toggle(UIPanelType.Bag);
    }
    public void ToggleShopPanel()
    {
        if (uiPanels[UIPanelType.Bag].activeSelf)
            Toggle(UIPanelType.Bag);

        Toggle(UIPanelType.Shop);
    }
    public void TogglePausePanel()
    {
        Toggle(UIPanelType.Pause);  // Abre o cierra el panel de pausa

        isPaused = !isPaused;
        VacuumSystem vacuumSystem = diver.GetComponent<VacuumSystem>();

        if (isPaused)
        {
            // Detener succi�n antes de desactivar
            if (vacuumSystem != null)
            {
                vacuumSystem.SetIsSucking(false);
                vacuumSystem.HandleSuctionSound();
            }
            Time.timeScale = 0f;
            vacuumSystem.enabled = false;
            surfaceZone.enabled = false;
        }
        else
        {
            Time.timeScale = 1f;
            vacuumSystem.enabled = true;
            surfaceZone.enabled = true;
        }
    }



}
