// BagButtonSimple.cs
using UnityEngine;

public class BagButtonSimple : MonoBehaviour
{
    public UIManager ui;                 // arrastra aquí tu UIManager en el Inspector

    // Llama esto desde el OnClick del botón (uGUI) o UI Toolkit callback
    public void OnClick()
    {
        if (ui == null) return;
        ui.Toggle(UIPanelType.Bag);
    }
}
