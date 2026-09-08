using System.Collections.Generic;
using ED262C;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // La Pila (Stack) obligatoria para el profesor
    private SimpleArrayStack<PanelUI> historyStack = new SimpleArrayStack<PanelUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Pequeño atajo guiño guiño
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    public void OpenPanelByID(string id) //el id hay que escribirlo en cada panel que tenga el script PanelUI
    {
        if (string.IsNullOrEmpty(id)) return;

        // Buscamos entre los paneles que existen en ESTA escena (activos e inactivos)
        PanelUI[] allPanels = FindObjectsByType<PanelUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        PanelUI targetPanel = null;
        foreach (PanelUI panel in allPanels)
        {
            if (panel.panelID == id)
            {
                targetPanel = panel;
                break;
            }
        }

        if (targetPanel != null)
        {
            OpenPanel(targetPanel);
        }
        else
        {
            Debug.LogWarning($"[UIManager] No se encontró ningún panel con ID '{id}' en esta escena papu.");
        }
    }


    /// Empuja un panel a la cima de la Pila (PUSH)

    public void OpenPanel(PanelUI newPanel)
    {
        if (newPanel == null) return;

        // Desactivamos el panel que estaba arriba actualmente sin sacarlo de la pila
        if (historyStack.Count > 0)
        {
            PanelUI currentPanel = historyStack.Peek();
            if (currentPanel != null)
            {
                currentPanel.ClosePanel();
            }
        }

        // PUSH: Agregamos el nuevo panel a la cima
        historyStack.Push(newPanel);
        newPanel.OpenPanel();
    }


    /// Desapila el panel actual y reactiva el anterior (POP & PEEK)

    public void GoBack()
    {
        // Si no hay nada o solo queda el panel base, no desapilamos
        if (historyStack.Count == 0)
        {
            Debug.Log("[Stack UI] No hay más paneles para volver atrás papu.");
            return;
        }

        //Sacamos el panel superior de la Pila y lo cerramos
        PanelUI topPanel = historyStack.Pop();

        if (topPanel != null)
        {
            topPanel.ClosePanel();
        }

        //Si todavía queda algún panel en la Pila, lo volvemos a mostrar
        PanelUI previousPanel;

        if (historyStack.Count > 0)
        {
            previousPanel = historyStack.Peek();
            if (previousPanel != null)
            {
                previousPanel.OpenPanel();
            }
        }
    }


    /// Vuelve al menú base limpiando todos los paneles emergentes de la Pila

    public void ClearToRoot()
    {
        while (historyStack.Count > 1)
        {
            PanelUI topPanel = historyStack.Pop();
            if (topPanel != null) topPanel.ClosePanel();
        }

        if (historyStack.Count == 1)
        {
            historyStack.Peek().OpenPanel();
        }
    }

    public void CloseGame() => Application.Quit();

    public void StartGame() => SceneManager.LoadScene("Miara");
}