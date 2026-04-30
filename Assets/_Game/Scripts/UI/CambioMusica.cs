using UnityEngine;

public class CambioMusica : MonoBehaviour
{
    [Header("Música de esta zona")]
    [Tooltip("Arrastra aquí la canción que debe sonar en este bioma")]
    [SerializeField] private AudioClip musicaBioma;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el que entra en la zona invisible es el jugador...
        if (collision.CompareTag("Player"))
        {
            // Le pedimos al mánager que cambie el disco
            if (AudioManager.Instancia != null && musicaBioma != null)
            {
                AudioManager.Instancia.ReproducirMusica(musicaBioma);
            }
        }
    }
}