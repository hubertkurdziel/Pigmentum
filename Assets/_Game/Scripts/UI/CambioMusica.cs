using UnityEngine;

public class CambioMusica : MonoBehaviour
{
    [Header("Configuración de Audio Local")]
    // Clip de audio específico que se activará al entrar en el volumen de colisión.
    [SerializeField] private AudioClip musicaBioma;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que entra en el disparador tiene la etiqueta Player.
        if (collision.CompareTag("Player"))
        {
            // Solicita al AudioManager el cambio de pista musical si las referencias son válidas.
            if (AudioManager.Instancia != null && musicaBioma != null)
            {
                AudioManager.Instancia.ReproducirMusica(musicaBioma);
            }
        }
    }
}