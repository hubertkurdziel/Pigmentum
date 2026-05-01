using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System.Collections; 

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string comicSceneName = "Comic"; 

    [Header("Sistema de Vídeo")]
    public VideoPlayer videoPlayer;
    public VideoClip clipIdle;
    public VideoClip clipPlay;
    public VideoClip clipOptions;
    public VideoClip clipQuit;

    [Header("Paneles y Navegación")]
    public GameObject panelMainMenu;
    public GameObject panelOptions;
    public GameObject firstButtonMainMenu; 
    public GameObject firstButtonOptions;  

    private void Start()
    {
        // Configura el estado inicial del puntero y despliega la interfaz principal[cite: 32].
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        // Alterna la visibilidad de los paneles y establece el foco de navegación inicial[cite: 32].
        panelMainMenu.SetActive(true);
        panelOptions.SetActive(false);

        StartCoroutine(SeleccionarBotonConRetraso(firstButtonMainMenu));

        QuitarResalte();
    }

    public void ShowOptions()
    {
        panelMainMenu.SetActive(false);
        panelOptions.SetActive(true);

        StartCoroutine(SeleccionarBotonConRetraso(firstButtonOptions));
    }

    public void ClickPlay()
    {
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
        Application.Quit(); 
    }

    // Actualiza el clip de vídeo en el reproductor según la selección actual de la interfaz[cite: 32].
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
        if (videoPlayer != null && clipIdle != null)
        {
            videoPlayer.clip = clipIdle;
            videoPlayer.Play();
        }
    }

    private IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        // Asegura que la selección del sistema de eventos ocurra después de la activación del panel[cite: 32].
        EventSystem.current.SetSelectedGameObject(null);
        yield return null; 
        
        if (boton != null)
        {
            EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}