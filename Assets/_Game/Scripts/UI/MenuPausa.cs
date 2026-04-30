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
    
    [Header("Navegación Mando/Teclado")]
    public GameObject primerBotonPausa; 
    public GameObject primerBotonOpciones; // <-- NUEVO: Arrastra aquí el primer botón de Ajustes

    private bool juegoPausado = false;
    private bool viendoOpciones = false; 

    private void Start()
    {
        if (panelBotonesPausa != null) panelBotonesPausa.SetActive(false);
        if (panelOpciones != null) panelOpciones.SetActive(false);
        if (fondoOscuro != null) fondoOscuro.SetActive(false); 
        Time.timeScale = 1f; 
    }

    private void Update()
    {
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

    // --- ACTUALIZADO: El botón de opciones ahora también da el foco ---
    public void AbrirOpciones()
    {
        panelBotonesPausa.SetActive(false); 
        panelOpciones.SetActive(true);      
        viendoOpciones = true;

        // <-- NUEVO: Le damos el foco al primer botón del panel de opciones
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

    private IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null; // Espera un frame para que el panel se active del todo
        EventSystem.current.SetSelectedGameObject(boton);
    }
}