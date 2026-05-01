using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    // Instancia estática para el acceso global al sistema de diálogos mediante el patrón Singleton[cite: 4].
    public static DialogueManager Instancia;

    [Header("Interfaz del Personaje No Jugador")]
    // Objeto que actúa como contenedor visual para los mensajes del NPC[cite: 4].
    public GameObject bocadilloNPC; 
    // Componente de texto para la renderización de los mensajes del NPC[cite: 4].
    public TextMeshProUGUI textoMensaje; 
    // Ajuste de posición relativo a la ubicación del NPC[cite: 4].
    public Vector3 offsetNPC = new Vector3(0, 1.5f, 0); 

    [Header("Interfaz del Jugador")]
    // Objeto contenedor para las opciones de respuesta del jugador[cite: 4].
    public GameObject bocadilloJugador; 
    // Referencia al transform del jugador para el seguimiento de la interfaz[cite: 4].
    public Transform transformJugador; 
    // Ajuste de posición relativo a la ubicación del jugador[cite: 4].
    public Vector3 offsetJugador = new Vector3(0, 1.5f, 0); 
    // Matriz de componentes de botón para las elecciones del diálogo[cite: 4].
    public Button[] botonesOpciones;
    // Matriz de componentes de texto asociados a los botones de opciones[cite: 4].
    public TextMeshProUGUI[] textosBotones;

    [Header("Configuración de Tiempos")]
    // Intervalo de tiempo entre la aparición de cada carácter durante la escritura[cite: 4].
    public float velocidadLetra = 0.05f;
    // Tiempo de espera adicional tras completar la escritura de una frase[cite: 4].
    public float tiempoLecturaExtra = 2f;

    private string[] frasesActuales;
    private int pasoActual = 0;
    private bool escribiendo = false;
    private bool esperandoEleccion = false;
    private bool esDialogoFinal = false; 

    private NPC_Dialogo npcActual;

    private void Awake() 
    { 
        // Asignación de la instancia Singleton en la inicialización del objeto[cite: 4].
        if (Instancia == null) Instancia = this; 
    }

    private void Update()
    {
        // Actualiza la posición del elemento visual del NPC para seguir su movimiento en pantalla[cite: 4].
        if (npcActual != null && bocadilloNPC.activeSelf)
        {
            bocadilloNPC.transform.position = npcActual.transform.position + offsetNPC;
        }

        // Actualiza la posición del elemento visual del jugador para seguir su movimiento en pantalla[cite: 4].
        if (transformJugador != null && bocadilloJugador.activeSelf)
        {
            bocadilloJugador.transform.position = transformJugador.position + offsetJugador;
        }
    }

    // Inicia la secuencia de diálogo procesando el saludo inicial o el menú de opciones del NPC[cite: 4].
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

    // Controla el flujo del diálogo, permitiendo completar frases o avanzar a la siguiente línea[cite: 4].
    public void AvanzarDialogo()
    {
        if (esperandoEleccion) return; 

        StopAllCoroutines();

        if (escribiendo)
        {
            // Completa el texto de forma instantánea si se solicita avanzar durante la escritura[cite: 4].
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
                // Finaliza la interacción o despliega el menú de opciones según el estado de la conversación[cite: 4].
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

    // Configura la visibilidad de los elementos para mostrar una nueva línea de texto[cite: 4].
    private void MostrarFrase()
    {
        bocadilloNPC.SetActive(true);
        bocadilloJugador.SetActive(false);
        
        StartCoroutine(EscribirLetraPorLetra(frasesActuales[pasoActual]));
    }

    // Corrutina para generar el efecto de aparición gradual del texto carácter por carácter[cite: 4].
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

    // Corrutina que introduce un retardo antes de proceder automáticamente a la siguiente frase[cite: 4].
    private IEnumerator EsperarYAvanzar()
    {
        yield return new WaitForSeconds(tiempoLecturaExtra);
        AvanzarDialogo();
    }

    // Gestiona la activación y configuración de los botones de elección en la interfaz del jugador[cite: 4].
    private void MostrarMenuOpciones()
    {
        if (npcActual.opciones == null || npcActual.opciones.Length == 0)
        {
            CerrarDialogo(); return;
        }

        esperandoEleccion = true;
        
        bocadilloNPC.SetActive(false); 
        bocadilloJugador.SetActive(true);

        // Limpieza de eventos previos y desactivación de botones no utilizados[cite: 4].
        foreach (var btn in botonesOpciones)
        {
            btn.onClick.RemoveAllListeners();
            btn.gameObject.SetActive(false);
        }

        GameObject primerBotonVisible = null; 

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (i < npcActual.opciones.Length)
            {
                // Omite opciones que ya han sido leídas si están configuradas para ocultarse[cite: 4].
                if (npcActual.opciones[i].ocultarTrasLeer && npcActual.opcionesLeidas[i]) continue; 

                botonesOpciones[i].gameObject.SetActive(true);
                textosBotones[i].text = npcActual.opciones[i].textoDelBoton;
                
                int indiceElegido = i; 
                botonesOpciones[i].onClick.AddListener(() => ProcesarEleccion(indiceElegido));

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
            // Asignación del foco del sistema de eventos al primer botón disponible para compatibilidad con mandos[cite: 4].
            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(primerBotonVisible);
        }
    }

    // Ejecuta la lógica asociada a la opción seleccionada por el jugador[cite: 4].
    public void ProcesarEleccion(int indice)
    {
        NPC_Dialogo.OpcionDialogo opcionElegida = npcActual.opciones[indice];
        npcActual.MarcarOpcionLeida(indice);
        esperandoEleccion = false;

        if (opcionElegida.esOpcionDeSalida)
        {
            // Procesa la respuesta final antes de cerrar la interacción[cite: 4].
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
            // Continúa la conversación o regresa al menú de opciones[cite: 4].
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

    // Desactiva la interfaz de diálogo y restablece el estado de los componentes relacionados[cite: 4].
    private void CerrarDialogo()
    {
        bocadilloNPC.SetActive(false);
        bocadilloJugador.SetActive(false);
        
        npcActual = null;
        esperandoEleccion = false;

        // Libera la selección del sistema de eventos para devolver el control al movimiento estándar[cite: 4].
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Verifica si hay alguna ventana de diálogo activa actualmente[cite: 4].
    public bool DialogoActivo() 
    { 
        return bocadilloNPC.activeSelf || bocadilloJugador.activeSelf; 
    }
}