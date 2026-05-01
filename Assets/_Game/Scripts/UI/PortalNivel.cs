using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalNivel : MonoBehaviour
{
    [SerializeField] private string nombreEscenaDestino;
    [SerializeField] private AudioClip sfxEntrar;

    // Gestiona la transición de escena y la reproducción de audio al detectar la colisión con el jugador.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (AudioManager.Instancia != null && sfxEntrar != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxEntrar);
            }
            
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}