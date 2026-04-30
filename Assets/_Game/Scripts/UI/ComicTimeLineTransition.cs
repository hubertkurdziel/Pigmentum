using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables; // ¡Obligatorio para usar Timelines en código!

public class ComicTimelineTransition : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("El nombre EXACTO de la escena de tu Nivel 1")]
    public string level1SceneName = "Level1";

    private PlayableDirector _director;

    private void Start()
    {
        // Buscamos el componente PlayableDirector en este mismo objeto
        _director = GetComponent<PlayableDirector>();

        if (_director != null)
        {
            // Nos suscribimos al evento: "Cuando termines, llama a la función CambiarEscena"
            _director.stopped += CambiarEscena;
        }
        else
        {
            Debug.LogWarning("¡No se ha encontrado un PlayableDirector en este objeto!");
        }
    }

    private void CambiarEscena(PlayableDirector pd)
    {
        // Cargamos el Nivel 1
        SceneManager.LoadScene(level1SceneName);
    }

    private void OnDestroy()
    {
        // Es una buena práctica limpiar el evento si la escena se destruye
        if (_director != null)
        {
            _director.stopped -= CambiarEscena;
        }
    }
}