using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton para llamarlo desde cualquier script (AudioManager.Instancia...)
    public static AudioManager Instancia;

    [Header("Altavoces (Audio Sources)")]
    [Tooltip("El altavoz para los efectos cortos (saltos, golpes, dash)")]
    [SerializeField] private AudioSource sfxSource;
    
    [Tooltip("El altavoz para la música de fondo (en bucle)")]
    [SerializeField] private AudioSource musicaSource;

    private void Awake()
    {
        // Configuración para que sobreviva al cambiar de nivel
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

    // ── MÉTODOS PÚBLICOS PARA QUE LOS USEN OTROS SCRIPTS ──

    /// <summary>
    /// Reproduce un sonido corto que se puede superponer con otros (Efectos)
    /// </summary>
    public void ReproducirSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            Debug.Log("🔊 AudioManager: Reproduciendo con éxito -> " + clip.name);
            sfxSource.PlayOneShot(clip); 
        }
        else if (sfxSource == null)
        {
            Debug.LogError("❌ AudioManager: ¡No tengo altavoz asignado en el Inspector!");
        }
    }

    /// <summary>
    /// Cambia la música de fondo
    /// </summary>
    public void ReproducirMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica != null && musicaSource != null)
        {
            // ¡NUEVO!: Solo la cambiamos si es diferente a la que ya está sonando
            if (musicaSource.clip == nuevaMusica) return; 

            musicaSource.clip = nuevaMusica;
            musicaSource.loop = true; 
            musicaSource.Play();
        }
    }

    public void DetenerSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }
}