using UnityEngine;

public class BolaVida : MonoBehaviour
{
    [Header("Ajustes de la Bola")]
    [SerializeField] private int cantidadCura = 1;
    [SerializeField] private float fuerzaSalto = 6f;

    [Header("Efectos")]
    [SerializeField] private AudioClip sfxRecogerVida;

    private void Start()
    {
        // Aplica un impulso físico inicial con una dirección horizontal aleatoria al instanciar el objeto[cite: 18].
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
        // Gestiona la restauración de puntos de salud y la activación de la secuencia sonora al contacto con el usuario[cite: 18].
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController scriptJugador = collision.gameObject.GetComponent<PlayerController>();
            
            if (scriptJugador != null)
            {
                scriptJugador.Curar(cantidadCura);
                
                if (AudioManager.Instancia != null && sfxRecogerVida != null)
                {
                    AudioManager.Instancia.ReproducirSFX(sfxRecogerVida);
                }
                
                Destroy(gameObject);
            }
        }
    }
}