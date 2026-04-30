using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float tiempoDeVida = 3f;
    
    [Header("Ajuste Visual")]
    [Tooltip("Prueba con 90, 180, 270 o -90 hasta que apunte perfecto")]
    [SerializeField] private float compensacionRotacion = 135f; 

    [Header("Efectos")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip sfxExplosion; // <-- NUEVO: Sonido de explosión

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, tiempoDeVida); 
    }

    public void SetDireccion(Vector2 direccion)
    {
        if (_rb != null)
        {
            _rb.linearVelocity = direccion.normalized * velocidad;
        }

        float anguloBase = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, anguloBase + compensacionRotacion);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignoramos si choca con otro enemigo
        if (collision.CompareTag("Enemigo")) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TomarDano(1, transform.position); 
            }
        }

        // Si choca con el suelo o el jugador, explota
        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("floorLayer") || !collision.isTrigger)
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // --- NUEVO: Reproducimos el sonido de explosión ---
            if (AudioManager.Instancia != null && sfxExplosion != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxExplosion);
            }

            Destroy(gameObject); 
        }
    }
}