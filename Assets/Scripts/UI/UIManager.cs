using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<Canvas> uiPanels;

    void Start()
    {
        foreach (var panel in uiPanels)
        {
            panel.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            uiPanels[0].enabled = !uiPanels[0].enabled;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            uiPanels[1].enabled = !uiPanels[1].enabled;
        }
    }
}
