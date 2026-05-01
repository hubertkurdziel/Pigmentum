using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Instancia estática para el acceso global al sistema de audio mediante el patrón Singleton.
    public static AudioManager Instancia;

    [Header("Configuración de Fuentes de Audio")]
    // Referencia al componente AudioSource destinado a la reproducción de efectos de sonido.
    [SerializeField] private AudioSource sfxSource;
    
    // Referencia al componente AudioSource destinado a la reproducción de música de fondo.
    [SerializeField] private AudioSource musicaSource;

    private void Awake()
    {
        // Implementación del patrón Singleton y persistencia entre escenas.
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

    // Ejecuta un clip de audio de forma no interrumpida, permitiendo la superposición.
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            Debug.Log("AudioManager: Reproduciendo clip -> " + clip.name);
            sfxSource.PlayOneShot(clip); 
        }
        else if (sfxSource == null)
        {
            Debug.LogError("AudioManager: El componente sfxSource no está asignado.");
        }
    }

    // Actualiza el clip de música actual y activa la reproducción en bucle.
    public void ReproducirMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica != null && musicaSource != null)
        {
            // Evita reiniciar la pista si el clip solicitado ya se encuentra en reproducción.
            if (musicaSource.clip == nuevaMusica) return; 

            musicaSource.clip = nuevaMusica;
            musicaSource.loop = true; 
            musicaSource.Play();
        }
    }

    // Detiene cualquier sonido que se esté reproduciendo en la fuente de efectos.
    public void DetenerSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }
}