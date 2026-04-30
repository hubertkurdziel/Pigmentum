using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Esto nos permite llamar al GameManager desde cualquier otro script usando GameManager.Instancia
    public static GameManager Instancia;

    [Header("Memoria de Checkpoints")]
    public Vector2 ultimoHardCheckpoint;
    public Vector2 ultimoSoftCheckpoint;

    [Header("Gestión de Vidas")]
    public int vidasMaximas = 3;
    public int vidasActuales;

    private void Awake()
    {
        // Configuramos el Singleton para que no se destruya al cambiar de nivel
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        vidasActuales = vidasMaximas;
    }

    // ── FUNCIONES PARA GUARDAR MEMORIA ──

    public void GuardarHardCheckpoint(Vector2 nuevaPosicion)
    {
        ultimoHardCheckpoint = nuevaPosicion;
        ultimoSoftCheckpoint = nuevaPosicion; // Al tocar una hoguera, también es tu zona segura inmediata
        vidasActuales = vidasMaximas;         // Recarga de vidas completas
        Debug.Log("🔥 HARD CHECKPOINT GUARDADO: " + nuevaPosicion);
    }

    public void GuardarSoftCheckpoint(Vector2 nuevaPosicion)
    {
        ultimoSoftCheckpoint = nuevaPosicion;
        Debug.Log("📍 Soft Checkpoint actualizado: " + nuevaPosicion);
    }
}