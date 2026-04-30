using System.Collections;
using UnityEngine;

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
    [SerializeField] private AudioSource audioSourceHoguera; // <-- NUEVO
    [SerializeField] private AudioClip sfxFuegoEncendido;    // <-- NUEVO

    private bool _yaActivado = false;

    private void Start()
    {
        if (spriteRenderer != null && spriteApagada != null)
        {
            spriteRenderer.sprite = spriteApagada;
        }

        if (particulasHumo != null)
        {
            particulasHumo.Stop();
        }

        // Nos aseguramos de que no suene nada al empezar apagada
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
                GameManager.Instancia.GuardarHardCheckpoint(puntoDeAparicion.position);
            }

            if (particulasHumo != null)
            {
                particulasHumo.Play();
            }

            // REPRODUCIR SONIDO DEL FUEGO
            if (audioSourceHoguera != null && sfxFuegoEncendido != null)
            {
                audioSourceHoguera.clip = sfxFuegoEncendido;
                audioSourceHoguera.loop = true; // Para que no pare de sonar
                audioSourceHoguera.Play();
            }

            StartCoroutine(AnimarFuego());
        }
    }

    private IEnumerator AnimarFuego()
    {
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