using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class ComicTimelineTransition : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string level1SceneName = "Level1";

    private PlayableDirector _director;

    private void Start()
    {
        _director = GetComponent<PlayableDirector>();

        // Suscribe el método de cambio de escena al evento de finalización del PlayableDirector[cite: 29].
        if (_director != null)
        {
            _director.stopped += CambiarEscena;
        }
    }

    private void CambiarEscena(PlayableDirector pd)
    {
        SceneManager.LoadScene(level1SceneName);
    }

    private void OnDestroy()
    {
        // Libera la suscripción al evento para evitar errores de referencia en memoria[cite: 29].
        if (_director != null)
        {
            _director.stopped -= CambiarEscena;
        }
    }
}