using UnityEngine;

public class ProyectilBoss : MonoBehaviour
{
    [Header("Ajustes del Proyectil")]
    public float velocidad = 8f;
    public int dano = 1;
    public float tiempoDeVida = 5f; 

    [Header("Efectos")]
    [SerializeField] private AudioClip sfxExplosion; 
    [SerializeField] private GameObject prefabExplosion;

    private void Start()
    {
        // Establece el tiempo de vida para la autodestrucción y aplica velocidad inicial.
        Destroy(gameObject, tiempoDeVida);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * velocidad;
        }
    }

    private void Explotar()
    {
        // Gestiona la reproducción de efectos sonoros, visuales y la eliminación del objeto.
        if (AudioManager.Instancia != null && sfxExplosion != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxExplosion);
        }

        if (prefabExplosion != null)
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Procesa la interacción con el jugador para aplicar daño y filtra colisiones con superficies sólidas.
        if (collision.CompareTag("Enemigo")) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TomarDano(dano, transform.position);
            }
        }

        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("floorLayer") || !collision.isTrigger)
        {
            Explotar();
        }
    }
}