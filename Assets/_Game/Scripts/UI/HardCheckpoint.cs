using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HardCheckpoint : MonoBehaviour
{
    [Header("Configuración del Checkpoint")]
    [SerializeField] private Transform puntoDeAparicion; 
    
    [Header("Sprites de la Hoguera")]
    [SerializeField] private SpriteRenderer spriteRenderer; 
    [SerializeField] private Sprite spriteApagada;          
    [SerializeField] private Sprite[] spritesEncendida;     
    [SerializeField] private float velocidadAnimacion = 0.1f; 

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem particulasHumo; 

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSourceHoguera; 
    [SerializeField] private AudioClip sfxFuegoEncendido;    

    private bool _yaActivado = false;

    private void Start()
    {
        // Inicializa el estado visual y sonoro del objeto como desactivado[cite: 31].
        if (spriteRenderer != null && spriteApagada != null)
        {
            spriteRenderer.sprite = spriteApagada;
        }

        if (particulasHumo != null)
        {
            particulasHumo.Stop();
        }

        if (audioSourceHoguera != null)
        {
            audioSourceHoguera.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_yaActivado)
        {
            _yaActivado = true;
            
            if (GameManager.Instancia != null)
            {
                // AHORA MANDAMOS LA POSICIÓN Y EL NÚMERO DE LA ESCENA ACTUAL
                int escenaActual = SceneManager.GetActiveScene().buildIndex;
                GameManager.Instancia.GuardarHardCheckpoint(puntoDeAparicion.position, escenaActual);
            }

            if (particulasHumo != null)
            {
                particulasHumo.Play();
            }

            if (audioSourceHoguera != null && sfxFuegoEncendido != null)
            {
                audioSourceHoguera.clip = sfxFuegoEncendido;
                audioSourceHoguera.loop = true; 
                audioSourceHoguera.Play();
            }

            StartCoroutine(AnimarFuego());
        }
    }
    private IEnumerator AnimarFuego()
    {
        // Gestiona la secuencia de imágenes para crear el efecto de animación cíclica[cite: 31].
        int frameActual = 0;

        while (true)
        {
            if (spriteRenderer != null && spritesEncendida.Length > 0)
            {
                spriteRenderer.sprite = spritesEncendida[frameActual];
                
                frameActual++;
                if (frameActual >= spritesEncendida.Length)
                {
                    frameActual = 0; 
                }
            }
            
            yield return new WaitForSeconds(velocidadAnimacion);
        }
    }
}