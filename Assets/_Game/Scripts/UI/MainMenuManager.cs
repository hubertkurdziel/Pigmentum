using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System.Collections; // <-- IMPORTANTE AÑADIR ESTO

public class MainMenuManager : MonoBehaviour
{
[Header("Configuración de Escena")]
    public string comicSceneName = "Comic"; // <-- CAMBIO AQUÍ (Pon el nombre de tu escena)

    [Header("Sistema de Vídeo")]
    public VideoPlayer videoPlayer;
    public VideoClip clipIdle;
    public VideoClip clipPlay;
    public VideoClip clipOptions;
    public VideoClip clipQuit;

    [Header("Paneles y Navegación")]
    public GameObject panelMainMenu;
    public GameObject panelOptions;
    public GameObject firstButtonMainMenu; // El Btn_Play
    public GameObject firstButtonOptions;  // La pestaña de Gráficos o el Btn_Back

    private void Start()
    {
        // 1. Escondemos el ratón y lo bloqueamos para que no interfiera con el mando
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 2. Cargamos el menú
        ShowMainMenu();
    }

    // ─── CONTROL DE PANELES Y NAVEGACIÓN ───────────────────────────────────

    public void ShowMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelOptions.SetActive(false);

        // Llamamos a la corrutina en lugar de hacerlo directamente
        StartCoroutine(SeleccionarBotonConRetraso(firstButtonMainMenu));

        QuitarResalte();
    }

    public void ShowOptions()
    {
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);

        // Llamamos a la corrutina en lugar de hacerlo directamente
        StartCoroutine(SeleccionarBotonConRetraso(firstButtonOptions));
    }

    // ─── ACCIONES PRINCIPALES (ON CLICK) ───────────────────────────────────

    public void ClickPlay()
    {
        // Carga la escena del Cómic.
        if (!string.IsNullOrEmpty(comicSceneName))
        {
            SceneManager.LoadScene(comicSceneName);
        }
    }

    public void ClickOptions()
    {
        ShowOptions();
    }

    public void ClickQuit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esto solo funciona cuando el juego está exportado (build)
    }

    // ─── SISTEMA DE VÍDEO DINÁMICO (EVENT TRIGGERS) ────────────────────────

    public void ResaltarPlay()
    {
        if (videoPlayer != null && clipPlay != null)
        {
            videoPlayer.clip = clipPlay;
            videoPlayer.Play();
        }
    }

    public void ResaltarOptions()
    {
        if (videoPlayer != null && clipOptions != null)
        {
            videoPlayer.clip = clipOptions;
            videoPlayer.Play();
        }
    }

    public void ResaltarQuit()
    {
        if (videoPlayer != null && clipQuit != null)
        {
            videoPlayer.clip = clipQuit;
            videoPlayer.Play();
        }
    }

    public void QuitarResalte()
    {
        // Esta función la llaman los botones cuando quitas el ratón de encima (PointerExit o Deselect)
        if (videoPlayer != null && clipIdle != null)
        {
            videoPlayer.clip = clipIdle;
            videoPlayer.Play();
        }
    }
    private IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        // 1. Limpiamos la selección actual
        EventSystem.current.SetSelectedGameObject(null);
        
        // 2. ESPERAMOS UN FOTOGRAMA a que Unity termine de encender los paneles
        yield return null; 
        
        // 3. Ahora sí, le forzamos a seleccionar el botón
        if (boton != null)
        {
            EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}