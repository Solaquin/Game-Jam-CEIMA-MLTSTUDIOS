using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum UIPanelType
{
    Main,
    RescueAnimals,
    GoHome,
    Bag,
    Shop,
    NoOxygen
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



    private Dictionary<UIPanelType, GameObject> uiPanels;

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
        // Si NoOxygen está activo, bloquea las otras UIs
        bool noOxygenActive = uiPanels[UIPanelType.NoOxygen].activeSelf;

        if (noOxygenActive)
        {
            uiPanels[UIPanelType.Bag].SetActive(false);
            uiPanels[UIPanelType.RescueAnimals].SetActive(false);
            uiPanels[UIPanelType.GoHome].SetActive(false);
        }
        else
        {
            uiPanels[UIPanelType.RescueAnimals].SetActive(true);
        }


        if (surfaceZone.IsAtBase())
        {
            uiPanels[UIPanelType.GoHome].SetActive(true);


            if (Input.GetKeyDown(KeyCode.T))
            {
                if (uiPanels[UIPanelType.Bag].activeSelf)
                {
                    Toggle(UIPanelType.Bag);
                }
                Toggle(UIPanelType.Shop);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                if (uiPanels[UIPanelType.Shop].activeSelf)
                {
                    Toggle(UIPanelType.Shop);
                }
                Toggle(UIPanelType.Bag);
            }

        }
        else if(!surfaceZone.IsAtBase() && !noOxygenActive)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Toggle(UIPanelType.Bag);
            }
        }

        if (surfaceZone.IsAtBase() == false && uiPanels[UIPanelType.Shop].activeSelf)
        {
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

    


}
