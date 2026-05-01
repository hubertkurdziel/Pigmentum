using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia;

    [Header("Coordenadas de Reaparición")]
    public Vector2 ultimoHardCheckpoint;
    public Vector2 ultimoSoftCheckpoint;

    [Header("Estado de Salud")]
    public int vidasMaximas = 3;
    public int vidasActuales;
    
    [HideInInspector] public Vector3 posicionHoguera;
    [HideInInspector] public Vector3 posicionSoftCheckpoint;
    private int escenaHoguera = -1; // -1: sin hoguera

    [HideInInspector] public bool reaparecerEnPortal = false; 

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instancia == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        vidasActuales = vidasMaximas;
    }

    public void GuardarHardCheckpoint(Vector3 posicion, int escena)
    {
        posicionHoguera = posicion;
        escenaHoguera = escena;
    }

    public void GuardarSoftCheckpoint(Vector3 posicion)
    {
        posicionSoftCheckpoint = posicion;
        Debug.Log("Soft Checkpoint guardado en: " + posicion);
    }

    // Muerte normal
    public void ReaparecerEnHoguera()
    {
        if (escenaHoguera != -1)
        {
            SceneManager.LoadScene(escenaHoguera);
        }
        else
        {
            SceneManager.LoadScene("Nivel 1"); 
        }
    }

    // Muerte en el Boss
    public void MorirEnBoss()
    {
        reaparecerEnPortal = true; 
        SceneManager.LoadScene("Nivel 1"); 
    }

    // Evento al cargar escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (reaparecerEnPortal && scene.name == "Nivel 1") 
        {
            GameObject puntoRespawn = GameObject.FindGameObjectWithTag("RespawnBoss");
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (puntoRespawn != null && player != null)
            {
                player.transform.position = puntoRespawn.transform.position;
                vidasActuales = vidasMaximas; 
            }
            else
            {
                Debug.LogWarning("No se encontró un objeto con el Tag 'RespawnBoss' o el 'Player' en la escena Nivel 1.");
            }
            
            reaparecerEnPortal = false; 
        }
    }
}