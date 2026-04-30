using UnityEngine;
using UnityEngine.SceneManagement; // <-- ¡Imprescindible para cambiar niveles!

public class PortalNivel : MonoBehaviour
{
    [Header("Configuración del Portal")]
    [SerializeField, Tooltip("Escribe el nombre EXACTO de la escena a la que quieres ir")] 
    private string nombreEscenaDestino;

    [Header("Efectos de Sonido")]
    [SerializeField, Tooltip("Sonido al entrar en el portal")]
    private AudioClip sfxEntrar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto que ha tocado el portal es nuestro jugador
        if (collision.CompareTag("Player"))
        {
            // Reproducimos el sonido a través del mánager global
            if (AudioManager.Instancia != null && sfxEntrar != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxEntrar);
            }
            
            // Le decimos a Unity que cargue el nivel destino
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}