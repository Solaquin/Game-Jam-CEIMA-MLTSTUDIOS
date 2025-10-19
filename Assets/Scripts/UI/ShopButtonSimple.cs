// ShopButtonSimple.cs (versión segura)
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonSimple : MonoBehaviour
{
    public UIManager ui;                 // arrastra tu UIManager
    public SurfaceZone surfaceZone;      // arrastra tu SurfaceZone
    public Button shopButton;            // arrastra el componente Button del mismo GO
    public CanvasGroup canvasGroup;      // opcional, para ocultar visualmente (asignar si lo usas)
    public bool hideWhenUnderwater = true; // si true, oculto visualmente pero NO desactivo el GO

    void Reset()
    {
        shopButton = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!surfaceZone) return;

        bool atBase = surfaceZone.IsAtBase();

        // 1) Habilitar/deshabilitar la interacción del botón
        if (shopButton) shopButton.interactable = atBase;

        // 2) Opcional: ocultar visualmente con CanvasGroup (sin apagar el GameObject)
        if (canvasGroup && hideWhenUnderwater)
        {
            canvasGroup.alpha = atBase ? 1f : 0f;
            canvasGroup.blocksRaycasts = atBase;
            canvasGroup.interactable = atBase;
        }
        // Importante: NO usar SetActive(false) aquí
    }

    // Asigna este método al OnClick del botón
    public void OnClick()
    {
        if (!ui || !surfaceZone) return;
        if (!surfaceZone.IsAtBase()) return; // solo funciona afuera
        ui.Toggle(UIPanelType.Shop);
    }
}
