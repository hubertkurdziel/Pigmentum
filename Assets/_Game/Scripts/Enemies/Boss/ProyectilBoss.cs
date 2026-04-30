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
        Destroy(gameObject, tiempoDeVida);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * velocidad;
        }
    }

    private void Explotar()
    {
        // Reproducir sonido si lo hay
        if (AudioManager.Instancia != null && sfxExplosion != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxExplosion);
        }

        // Instanciar las partículas si hemos asignado el prefab
        if (prefabExplosion != null)
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignoramos si choca con otro enemigo (para que el boss no se dispare a sí mismo)
        if (collision.CompareTag("Enemigo")) return;

        // Si golpea al jugador
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TomarDano(dano, transform.position);
            }
        }

        // --- CAMBIO CLAVE ---
        // Explotar si choca con el jugador, con el layer "floorLayer", o con CUALQUIER objeto sólido (!isTrigger)
        // (Esto incluye tus objetos en el layer "Wall" porque seguramente tengan un Collider2D normal, no trigger)
        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("floorLayer") || !collision.isTrigger)
        {
            Explotar();
        }
    }
}