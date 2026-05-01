using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float tiempoDeVida = 3f;
    
    [Header("Ajuste Visual")]
    [SerializeField] private float compensacionRotacion = 135f; 

    [Header("Efectos")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip sfxExplosion;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, tiempoDeVida); 
    }

    // Establece el vector de movimiento y orienta el objeto aplicando el desfase de rotación configurado[cite: 12].
    public void SetDireccion(Vector2 direccion)
    {
        if (_rb != null)
        {
            _rb.linearVelocity = direccion.normalized * velocidad;
        }

        float anguloBase = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, anguloBase + compensacionRotacion);
    }

    // Procesa el impacto con el usuario para aplicar daño y activa la destrucción del objeto al tocar superficies[cite: 12].
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo")) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TomarDano(1, transform.position); 
            }
        }

        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("floorLayer") || !collision.isTrigger)
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            if (AudioManager.Instancia != null && sfxExplosion != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxExplosion);
            }

            Destroy(gameObject); 
        }
    }
}