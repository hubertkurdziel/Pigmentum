using UnityEngine;

public class ProyectilPerseguidor : MonoBehaviour
{
    [Header("Ajustes")]
    public float velocidad = 5f;
    public float velocidadRotacion = 200f; 
    public float tiempoDeVida = 5f;
    
    [Header("Efectos")]
    public GameObject explosionPrefab; 
    public AudioClip sfxExplosion; // <-- NUEVO: Sonido de explosión
    
    private Transform _player;
    private Rigidbody2D _rb;
    private bool _desviado = false;
    private Transform _creador;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Invoke("Explotar", tiempoDeVida); 
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;
    }

    public void Configurar(Transform creador)
    {
        _creador = creador;
    }

    public void Desviar()
    {
        _desviado = true;
        _rb.angularVelocity = 0f; 
    }

    private void FixedUpdate()
    {
        if (_desviado)
        {
            if (_creador != null)
            {
                Vector2 direccionVenganza = (_creador.position - transform.position).normalized;
                _rb.linearVelocity = direccionVenganza * (velocidad * 1.5f); 
                
                float angulo = Mathf.Atan2(direccionVenganza.y, direccionVenganza.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angulo);
            }
            else 
            {
                _rb.linearVelocity = transform.right * (velocidad * 1.5f);
            }
            return; 
        }

        if (_player != null)
        {
            Vector2 direccion = (Vector2)_player.position - _rb.position;
            direccion.Normalize();
            
            float rotacionZ = Vector3.Cross(direccion, transform.right).z;
            _rb.angularVelocity = -rotacionZ * velocidadRotacion;
            _rb.linearVelocity = transform.right * velocidad;
        }
    }

    private void Explotar()
    {
        // 1. Instanciar partículas
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        // 2. Reproducir sonido a través del mánager global
        if (AudioManager.Instancia != null && sfxExplosion != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxExplosion);
        }
        
        // 3. Destruir el objeto
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_desviado)
        {
            if (collision.CompareTag("Enemigo"))
            {
                EnemigoPared pared = collision.GetComponent<EnemigoPared>();
                if (pared != null) pared.RecibirDano(1, transform.position);

                FungnatIA fungnat = collision.GetComponent<FungnatIA>();
                if (fungnat != null) fungnat.RecibirDano(1, transform.position);

                Explotar(); 
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null) player.TomarDano(1, transform.position);
                
                Explotar(); 
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Explotar(); 
        }
    }
}