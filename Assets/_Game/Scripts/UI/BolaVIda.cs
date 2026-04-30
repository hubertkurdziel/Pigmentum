using UnityEngine;

public class BolaVida : MonoBehaviour
{
    [Header("Ajustes de la Bola")]
    [SerializeField, Tooltip("Cuánta vida recupera")] 
    private int cantidadCura = 1;
    [SerializeField, Tooltip("Fuerza con la que salta al salir del enemigo")] 
    private float fuerzaSalto = 6f;

    [Header("Efectos")]
    [SerializeField, Tooltip("Sonido al recoger la vida")]
    private AudioClip sfxRecogerVida; // <-- Aquí guardamos tu sonido

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direccionX = Random.Range(-1f, 1f);
            Vector2 direccionSalto = new Vector2(direccionX, 1.5f).normalized;
            rb.AddForce(direccionSalto * fuerzaSalto, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController scriptJugador = collision.gameObject.GetComponent<PlayerController>();
            
            if (scriptJugador != null)
            {
                scriptJugador.Curar(cantidadCura);
                
                // Le pasamos el sonido al AudioManager justo antes de desaparecer
                if (AudioManager.Instancia != null && sfxRecogerVida != null)
                {
                    AudioManager.Instancia.ReproducirSFX(sfxRecogerVida);
                }
                
                Destroy(gameObject);
            }
        }
    }
}