using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class MenuOpciones : MonoBehaviour
{
    [Header("Conexión Principal")]
    [SerializeField] private MainMenuManager menuPrincipal;
    [SerializeField] private MenuPausa menuPausa; 
    [Header("Navegación Mando/Teclado")]
    [SerializeField] private GameObject primerBotonOpciones; 

    [Header("Paneles de Pestañas")]
    [SerializeField] private GameObject panelGraficos;
    [SerializeField] private GameObject panelAudio;
    [SerializeField] private GameObject panelControles;

    [Header("Controles de Gráficos (Imágenes)")]
    [SerializeField] private Image imagenResolucion; 
    [SerializeField] private Sprite[] spritesResolucion; 
    [SerializeField] private Image imagenModoPantalla; 
    [SerializeField] private Sprite[] spritesModoPantalla; 
    [SerializeField] private Image imagenFps;          
    [SerializeField] private Sprite[] spritesFps; 
    [SerializeField] private Toggle mostrarFpsToggle;
    [SerializeField] private Image imagenBrillo; 
    [SerializeField] private Sprite[] spritesBrillo; 
    [SerializeField] private Image capaOscuraBrillo;

    [Header("Controles de Audio (Imágenes)")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image imagenVolMaster;
    [SerializeField] private Image imagenVolMusica;
    [SerializeField] private Image imagenVolEfectos;
    [SerializeField] private Sprite[] spritesVolumen; 

    [Header("Configuración Especial Toggle")]
    [SerializeField] private Sprite spriteToggleActivado;    
    [SerializeField] private Sprite spriteToggleDesactivado; 
    [SerializeField] private Image imagenFondoToggle;

    [Header("Fondo de Vídeo")]
    [SerializeField] private VideoPlayer fondoVideo;

    private int resolucionIndex = 1;
    private int modoPantallaIndex = 0;
    private int fpsIndex = 1; 
    private int brilloIndex;
    private int[] valoresFps = { 30, 60, 120, 144, -1 };
    private int volMasterIndex = 10;
    private int volMusicaIndex = 10;
    private int volEfectosIndex = 10;
    private bool cargandoDatos = false;

    private void Start()
    {   
        // Inicializa el estado de las configuraciones cargando valores persistentes[cite: 34].
        MostrarPanelGraficos(); 
        CargarDatosGuardados();

        AplicarVolumenAlMixer("VolumenMaster", volMasterIndex);
        AplicarVolumenAlMixer("VolumenMusica", volMusicaIndex);
        AplicarVolumenAlMixer("VolumenEfectos", volEfectosIndex);

        if (!PlayerPrefs.HasKey("PrimerInicioCompletado"))
        {
            AplicarYGuardarCambios(); 
            PlayerPrefs.SetInt("PrimerInicioCompletado", 1); 
            PlayerPrefs.Save();
        }
    }

    private void OnEnable()
    {
        MostrarPanelGraficos();

        EventSystem.current.SetSelectedGameObject(null);
        if (primerBotonOpciones != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonOpciones);
        }
    }

    private void CargarDatosGuardados()
    {
        // Recupera los índices de configuración almacenados y actualiza los elementos visuales[cite: 34].
        cargandoDatos = true;

        resolucionIndex = PlayerPrefs.GetInt("Resolucion", 1); 
        ActualizarImagenResolucion(); 

        modoPantallaIndex = PlayerPrefs.GetInt("ModoPantalla", 0);
        ActualizarImagenModoPantalla();

        fpsIndex = PlayerPrefs.GetInt("LimiteFPS", 1);
        ActualizarImagenFps();

        if(mostrarFpsToggle != null) 
            mostrarFpsToggle.isOn = PlayerPrefs.GetInt("MostrarFPS", 0) == 1;
        
        brilloIndex = PlayerPrefs.GetInt("BrilloIndex", spritesBrillo.Length - 1); 
        ActualizarImagenBrillo(); 
        AplicarBrilloVisual();

        volMasterIndex = PlayerPrefs.GetInt("VolMaster", 10);
        volMusicaIndex = PlayerPrefs.GetInt("VolMusica", 10);
        volEfectosIndex = PlayerPrefs.GetInt("VolEfectos", 10);
        
        ActualizarImagenesAudio();
        
        if(audioMixer != null) {
            AplicarVolumenAlMixer("VolumenMaster", volMasterIndex);
            AplicarVolumenAlMixer("VolumenMusica", volMusicaIndex);
            AplicarVolumenAlMixer("VolumenEfectos", volEfectosIndex);
        }

        cargandoDatos = false;
    }

    public void SiguienteResolucion() { resolucionIndex++; if (resolucionIndex >= spritesResolucion.Length) resolucionIndex = 0; ActualizarImagenResolucion(); }
    public void AnteriorResolucion() { resolucionIndex--; if (resolucionIndex < 0) resolucionIndex = spritesResolucion.Length - 1; ActualizarImagenResolucion(); }
    private void ActualizarImagenResolucion() { if(imagenResolucion != null && spritesResolucion.Length > 0) { resolucionIndex = Mathf.Clamp(resolucionIndex, 0, spritesResolucion.Length - 1); imagenResolucion.sprite = spritesResolucion[resolucionIndex]; } }

    public void SiguienteModoPantalla() { modoPantallaIndex++; if (modoPantallaIndex >= spritesModoPantalla.Length) modoPantallaIndex = 0; ActualizarImagenModoPantalla(); }
    public void AnteriorModoPantalla() { modoPantallaIndex--; if (modoPantallaIndex < 0) modoPantallaIndex = spritesModoPantalla.Length - 1; ActualizarImagenModoPantalla(); }
    private void ActualizarImagenModoPantalla() { if(imagenModoPantalla != null && spritesModoPantalla.Length > 0) { modoPantallaIndex = Mathf.Clamp(modoPantallaIndex, 0, spritesModoPantalla.Length - 1); imagenModoPantalla.sprite = spritesModoPantalla[modoPantallaIndex]; } }

    public void SiguienteFps() { fpsIndex++; if (fpsIndex >= spritesFps.Length) fpsIndex = 0; ActualizarImagenFps(); }
    public void AnteriorFps() { fpsIndex--; if (fpsIndex < 0) fpsIndex = spritesFps.Length - 1; ActualizarImagenFps(); }
    private void ActualizarImagenFps() { if(imagenFps != null && spritesFps.Length > 0) { fpsIndex = Mathf.Clamp(fpsIndex, 0, spritesFps.Length - 1); imagenFps.sprite = spritesFps[fpsIndex]; } }

    public void SiguienteBrillo() { brilloIndex++; if (brilloIndex >= spritesBrillo.Length) brilloIndex = 0; ActualizarImagenBrillo(); }
    public void AnteriorBrillo() { brilloIndex--; if (brilloIndex < 0) brilloIndex = spritesBrillo.Length - 1; ActualizarImagenBrillo(); }
    private void ActualizarImagenBrillo() { if (imagenBrillo != null && spritesBrillo.Length > 0) { brilloIndex = Mathf.Clamp(brilloIndex, 0, spritesBrillo.Length - 1); imagenBrillo.sprite = spritesBrillo[brilloIndex]; } }
    private void AplicarBrilloVisual() { if (capaOscuraBrillo != null && spritesBrillo.Length > 1) { float valorLinear = Mathf.Lerp(0.2f, 1f, (float)brilloIndex / (spritesBrillo.Length - 1)); Color c = capaOscuraBrillo.color; c.a = 1f - valorLinear; capaOscuraBrillo.color = c; } }

    public void ToggleFPS(bool activo) 
    { 
        if (cargandoDatos) return; 

        if (imagenFondoToggle != null)
        {
            imagenFondoToggle.sprite = activo ? spriteToggleActivado : spriteToggleDesactivado;
        }
    }

    public void SiguienteVolMaster() { if (volMasterIndex < 10) { volMasterIndex++; AlCambiarAudio("VolumenMaster"); } }
    public void AnteriorVolMaster()  { if (volMasterIndex > 0)  { volMasterIndex--; AlCambiarAudio("VolumenMaster"); } }

    public void SiguienteVolMusica() { if (volMusicaIndex < 10) { volMusicaIndex++; AlCambiarAudio("VolumenMusica"); } }
    public void AnteriorVolMusica()  { if (volMusicaIndex > 0)  { volMusicaIndex--; AlCambiarAudio("VolumenMusica"); } }

    public void SiguienteVolEfectos() { if (volEfectosIndex < 10) { volEfectosIndex++; AlCambiarAudio("VolumenEfectos"); } }
    public void AnteriorVolEfectos()  { if (volEfectosIndex > 0)  { volEfectosIndex--; AlCambiarAudio("VolumenEfectos"); } }

    private void AlCambiarAudio(string parametro) 
    { 
        ActualizarImagenesAudio(); 
    }

    private void ActualizarImagenesAudio() 
    { 
        if (spritesVolumen.Length > 0)
        {
            if (imagenVolMaster != null) imagenVolMaster.sprite = spritesVolumen[Mathf.Clamp(volMasterIndex, 0, spritesVolumen.Length - 1)];
            if (imagenVolMusica != null) imagenVolMusica.sprite = spritesVolumen[Mathf.Clamp(volMusicaIndex, 0, spritesVolumen.Length - 1)];
            if (imagenVolEfectos != null) imagenVolEfectos.sprite = spritesVolumen[Mathf.Clamp(volEfectosIndex, 0, spritesVolumen.Length - 1)];
        }
    }

    // Convierte el valor de volumen linear a escala logarítmica de decibelios para el mezclador de audio[cite: 34].
    private void AplicarVolumenAlMixer(string parametro, int index) 
    { 
        if (audioMixer == null) return; 
        float volumenLinear = index / 10f; 
        float decibelios = (volumenLinear <= 0.001f) ? -80f : Mathf.Log10(volumenLinear) * 20f; 
        audioMixer.SetFloat(parametro, decibelios); 
    }

    // Ejecuta y almacena permanentemente todos los ajustes de hardware y software seleccionados[cite: 34].
    public void AplicarYGuardarCambios()
    {
        if (fondoVideo != null) fondoVideo.Pause();

        int ancho = 1920;
        int alto = 1080;

        if (resolucionIndex == 0) { ancho = 1280; alto = 720; }
        else if (resolucionIndex == 1) { ancho = 1920; alto = 1080; }
        else if (resolucionIndex == 2) { ancho = 2560; alto = 1440; }

        FullScreenMode modo = FullScreenMode.ExclusiveFullScreen;
        if (modoPantallaIndex == 1) modo = FullScreenMode.Windowed;
        else if (modoPantallaIndex == 2) modo = FullScreenMode.FullScreenWindow;
        
        Screen.SetResolution(ancho, alto, modo);

        if (fondoVideo != null) fondoVideo.Play();
        
        Application.targetFrameRate = valoresFps[Mathf.Clamp(fpsIndex, 0, valoresFps.Length - 1)];

        AplicarVolumenAlMixer("VolumenMaster", volMasterIndex);
        AplicarVolumenAlMixer("VolumenMusica", volMusicaIndex);
        AplicarVolumenAlMixer("VolumenEfectos", volEfectosIndex);

        AplicarBrilloVisual();

        if (mostrarFpsToggle != null) 
        {
            PlayerPrefs.SetInt("MostrarFPS", mostrarFpsToggle.isOn ? 1 : 0);
        }

        ContadorFPS contador = FindObjectOfType<ContadorFPS>();
        if (contador != null) 
        { 
            contador.ActualizarEstado(); 
        }

        PlayerPrefs.SetInt("Resolucion", resolucionIndex);
        PlayerPrefs.SetInt("ModoPantalla", modoPantallaIndex);
        PlayerPrefs.SetInt("LimiteFPS", fpsIndex);
        PlayerPrefs.SetInt("BrilloIndex", brilloIndex);
        PlayerPrefs.SetInt("VolMaster", volMasterIndex);
        PlayerPrefs.SetInt("VolMusica", volMusicaIndex);
        PlayerPrefs.SetInt("VolEfectos", volEfectosIndex);
        
        PlayerPrefs.Save(); 
    }

    public void SalirDeOpciones() 
    { 
        CargarDatosGuardados(); 
        
        if (menuPrincipal != null) menuPrincipal.ShowMainMenu(); 
        if (menuPausa != null) menuPausa.VolverDeOpciones(); 
    }

    public void MostrarPanelGraficos() { if(panelGraficos != null) panelGraficos.SetActive(true); if(panelAudio != null) panelAudio.SetActive(false); if(panelControles != null) panelControles.SetActive(false); }
    public void MostrarPanelAudio() { if(panelGraficos != null) panelGraficos.SetActive(false); if(panelAudio != null) panelAudio.SetActive(true); if(panelControles != null) panelControles.SetActive(false); }
    public void MostrarPanelControles() { if(panelGraficos != null) panelGraficos.SetActive(false); if(panelAudio != null) panelAudio.SetActive(false); if(panelControles != null) panelControles.SetActive(true); }
}