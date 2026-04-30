using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; // <-- NUEVO: Imprescindible para el mando

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instancia;

    [Header("Bocadillo Flotante del NPC")]
    public GameObject bocadilloNPC; 
    public TextMeshProUGUI textoMensaje; 
    public Vector3 offsetNPC = new Vector3(0, 1.5f, 0); 

    [Header("Bocadillo Flotante del Jugador")]
    public GameObject bocadilloJugador; 
    public Transform transformJugador; 
    public Vector3 offsetJugador = new Vector3(0, 1.5f, 0); 
    public Button[] botonesOpciones;
    public TextMeshProUGUI[] textosBotones;

    [Header("Ajustes")]
    public float velocidadLetra = 0.05f;
    public float tiempoLecturaExtra = 2f;

    private string[] frasesActuales;
    private int pasoActual = 0;
    private bool escribiendo = false;
    private bool esperandoEleccion = false;
    private bool esDialogoFinal = false; 

    private NPC_Dialogo npcActual;

    private void Awake() { if (Instancia == null) Instancia = this; }
    private void Update()
    {
        if (npcActual != null && bocadilloNPC.activeSelf)
        {
            bocadilloNPC.transform.position = npcActual.transform.position + offsetNPC;
        }

        if (transformJugador != null && bocadilloJugador.activeSelf)
        {
            bocadilloJugador.transform.position = transformJugador.position + offsetJugador;
        }
    }
    public void IniciarDialogoConOpciones(NPC_Dialogo npc)
    {
        npcActual = npc;
        esperandoEleccion = false;
        esDialogoFinal = false;

        if (npc.saludoInicial != null && npc.saludoInicial.Length > 0)
        {
            frasesActuales = npc.saludoInicial;
            pasoActual = 0;
            MostrarFrase();
        }
        else
        {
            MostrarMenuOpciones();
        }
    }

    public void AvanzarDialogo()
    {
        if (esperandoEleccion) return; 

        StopAllCoroutines();

        if (escribiendo)
        {
            textoMensaje.text = frasesActuales[pasoActual];
            escribiendo = false;
            if (npcActual != null) npcActual.SetHablando(false);
            
            StartCoroutine(EsperarYAvanzar()); 
        }
        else
        {
            pasoActual++;
            if (pasoActual < frasesActuales.Length) 
            {
                MostrarFrase();
            }
            else 
            {
                if (esDialogoFinal)
                {
                    npcActual.DespedirYDesactivar();
                    CerrarDialogo();
                }
                else
                {
                    MostrarMenuOpciones();
                }
            }
        }
    }

    private void MostrarFrase()
    {
        bocadilloNPC.SetActive(true);
        bocadilloJugador.SetActive(false);
        
        StartCoroutine(EscribirLetraPorLetra(frasesActuales[pasoActual]));
    }

    private IEnumerator EscribirLetraPorLetra(string frase)
    {
        escribiendo = true;
        textoMensaje.text = "";

        if (npcActual != null) npcActual.SetHablando(true);
        
        foreach (char letra in frase.ToCharArray())
        {
            textoMensaje.text += letra;
            yield return new WaitForSeconds(velocidadLetra);
        }
        
        escribiendo = false;
        if (npcActual != null) npcActual.SetHablando(false);

        StartCoroutine(EsperarYAvanzar());
    }

    private IEnumerator EsperarYAvanzar()
    {
        yield return new WaitForSeconds(tiempoLecturaExtra);
        AvanzarDialogo();
    }

    private void MostrarMenuOpciones()
    {
        if (npcActual.opciones == null || npcActual.opciones.Length == 0)
        {
            CerrarDialogo(); return;
        }

        esperandoEleccion = true;
        
        bocadilloNPC.SetActive(false); 
        bocadilloJugador.SetActive(true);

        foreach (var btn in botonesOpciones)
        {
            btn.onClick.RemoveAllListeners();
            btn.gameObject.SetActive(false);
        }

        GameObject primerBotonVisible = null; // <-- NUEVO: Para guardar el primer botón que encendamos

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (i < npcActual.opciones.Length)
            {
                if (npcActual.opciones[i].ocultarTrasLeer && npcActual.opcionesLeidas[i]) continue; 

                botonesOpciones[i].gameObject.SetActive(true);
                textosBotones[i].text = npcActual.opciones[i].textoDelBoton;
                
                int indiceElegido = i; 
                botonesOpciones[i].onClick.AddListener(() => ProcesarEleccion(indiceElegido));

                // Si es el primer botón que encendemos, lo guardamos
                if (primerBotonVisible == null)
                {
                    primerBotonVisible = botonesOpciones[i].gameObject;
                }
            }
        }

        if (primerBotonVisible == null)
        {
            CerrarDialogo();
        }
        else
        {
            // <-- NUEVO: Le forzamos a Unity a seleccionar este botón para que el mando empiece desde aquí
            EventSystem.current.SetSelectedGameObject(null); // Limpiamos por si acaso
            EventSystem.current.SetSelectedGameObject(primerBotonVisible);
        }
    }

    public void ProcesarEleccion(int indice)
    {
        NPC_Dialogo.OpcionDialogo opcionElegida = npcActual.opciones[indice];
        npcActual.MarcarOpcionLeida(indice);
        esperandoEleccion = false;

        if (opcionElegida.esOpcionDeSalida)
        {
            if (opcionElegida.frasesRespuesta != null && opcionElegida.frasesRespuesta.Length > 0)
            {
                esDialogoFinal = true; 
                frasesActuales = opcionElegida.frasesRespuesta;
                pasoActual = 0;
                MostrarFrase(); 
            }
            else
            {
                npcActual.DespedirYDesactivar();
                CerrarDialogo();
            }
        }
        else
        {
            esDialogoFinal = false;
            
            if (opcionElegida.frasesRespuesta != null && opcionElegida.frasesRespuesta.Length > 0)
            {
                frasesActuales = opcionElegida.frasesRespuesta;
                pasoActual = 0;
                MostrarFrase(); 
            }
            else MostrarMenuOpciones();
        }
    }

    private void CerrarDialogo()
    {
        bocadilloNPC.SetActive(false);
        bocadilloJugador.SetActive(false);
        
        npcActual = null;
        esperandoEleccion = false;

        // <-- NUEVO: Soltamos el botón para que el jugador pueda seguir moviéndose normal
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public bool DialogoActivo() 
    { 
        return bocadilloNPC.activeSelf || bocadilloJugador.activeSelf; 
    }
}