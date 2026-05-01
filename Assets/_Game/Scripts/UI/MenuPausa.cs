using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; 
using System.Collections; 

public class MenuPausa : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelBotonesPausa; 
    public GameObject panelOpciones;     
    public GameObject fondoOscuro;
    
    [Header("Navegación")]
    public GameObject primerBotonPausa; 
    public GameObject primerBotonOpciones; 

    private bool juegoPausado = false;
    private bool viendoOpciones = false; 

    private void Start()
    {
        // Inicializa el estado de la interfaz y asegura que el tiempo del motor sea normal[cite: 35].
        if (panelBotonesPausa != null) panelBotonesPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (fondoOscuro != null) fondoOscuro.SetActive(false); 
        Time.timeScale = 1f; 
    }

    private void Update()
    {
        // Detecta la entrada de teclado o mando para alternar el estado de pausa[cite: 35].
        bool pausaPulsada = false;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pausaPulsada = true;
        }

        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            pausaPulsada = true;
        }

        if (pausaPulsada)
        {
            if (juegoPausado)
            {
                if (viendoOpciones) VolverDeOpciones(); 
                else Reanudar(); 
            }
            else Pausar();
        }
    }

    // Detiene el tiempo del motor y habilita los componentes visuales del menú[cite: 35].
    public void Pausar()
    {
        panelBotonesPausa.SetActive(true);
        panelOpciones.SetActive(false);
        if (fondoOscuro != null) fondoOscuro.SetActive(true); 
        Time.timeScale = 0f;
        juegoPausado = true;
        viendoOpciones = false;

        if (primerBotonPausa != null)
        {
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonPausa));
        }
    }

    // Restablece la velocidad del tiempo y oculta la interfaz de usuario[cite: 35].
    public void Reanudar()
    {
        panelBotonesPausa.SetActive(false);
        panelOpciones.SetActive(false);
        if (fondoOscuro != null) fondoOscuro.SetActive(false); 
        Time.timeScale = 1f;
        juegoPausado = false;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Alterna la visibilidad entre el panel de pausa principal y el panel de configuración[cite: 35].
    public void AbrirOpciones()
    {
        panelBotonesPausa.SetActive(false); 
        panelOpciones.SetActive(true);      
        viendoOpciones = true;

        if (primerBotonOpciones != null)
        {
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonOpciones));
        }
    }

    public void VolverDeOpciones()
    {
        panelOpciones.SetActive(false);     
        panelBotonesPausa.SetActive(true);  
        viendoOpciones = false;

        if (primerBotonPausa != null)
        {
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonPausa));
        }
    }

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    // Garantiza que la selección del sistema de eventos se realice después de la activación del panel[cite: 35].
    private IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null; 
        EventSystem.current.SetSelectedGameObject(boton);
    }
}