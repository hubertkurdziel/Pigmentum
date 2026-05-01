using System.Collections;
using UnityEngine;

public class FungnatIA : MonoBehaviour
{
    [Header("Vida y Daño")]
    [SerializeField] private int vidaMaxima = 3;
    private int _vidaActual;
    private bool _esInvulnerable = false; 

    [Header("Drops (Recompensas)")]
    [SerializeField] private GameObject prefabBolaVida;
    [SerializeField, Range(0f, 100f)] private float probabilidadDropVida = 20f;
    [SerializeField] private GameObject prefabMonedaDorada;
    [SerializeField] private int minMonedasDrop = 1;
    [SerializeField] private int maxMonedasDrop = 3;

    [Header("Efectos de Impacto")]
    [SerializeField] private Material materialFlashBlanco; 
    [SerializeField] private GameObject particulasGolpePrefab;
    [SerializeField] private GameObject explosionMuertePrefab; 
    [SerializeField] private float fuerzaEmpuje = 8f; 
    private Material _materialOriginal; 

    [Header("Sonidos del Fungnat")]
    [SerializeField] private AudioClip sfxVolar;   
    [SerializeField] private AudioClip sfxAtacar;  
    [SerializeField] private AudioClip sfxMorir;   

    private AudioSource _audioSource;

    [Header("Referencias")]
    private Transform _player; 
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform puntoDisparo;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Rigidbody2D _rb; 

    [Header("Movimiento Orgánico (Vuelo)")]
    [SerializeField] private float velocidadVuelo = 3f;
    [SerializeField] private float radioVagabundeo = 4f; 
    [SerializeField] private float amplitudFlote = 0.5f; 
    [SerializeField] private float velocidadFlote = 2f;  
    [SerializeField] private float suavidadMovimiento = 0.3f; 
    
    [Header("Combate & Persecución")]
    [SerializeField] private float radioDeteccion = 12f; 
    [SerializeField] private float distanciaOptimaAtaque = 5f; 
    [SerializeField] private float cadenciaDisparo = 2.0f; 

    private Vector2 _posicionInicial;
    private Vector2 _puntoDestinoVagabundeo;
    private Vector2 _velocidadSuavizada; 
    private float _tiempoVagabundeo;
    private float _cronometroDisparo;
    private bool _jugadorDetectado = false;
    private float _puntoDisparoOffsetBaseX; 

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator       = GetComponent<Animator>();
        _rb             = GetComponent<Rigidbody2D>(); 
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        
        if (puntoDisparo != null) _puntoDisparoOffsetBaseX = puntoDisparo.localPosition.x;

        _vidaActual = vidaMaxima;
        if (_spriteRenderer != null) _materialOriginal = _spriteRenderer.material;

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null && sfxVolar != null)
        {
            _audioSource.clip = sfxVolar;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
    private void Start()
    {
        _posicionInicial = transform.position;
        ElegirNuevoDestinoVagabundeo();
    }

    // Evalúa el estado de detección para alternar entre el comportamiento de patrulla y el ataque[cite: 13].
    private void Update()
    {
        if (_player == null || _esInvulnerable) return; 

        VerificarDeteccionJugador();
        GirarSpriteHaciaDestino(); 

        if (_jugadorDetectado) ComportamientoAtaqueYPersecucion();
        else ComportamientoVagabundeo();
    }

    private void ComportamientoVagabundeo()
    {
        _tiempoVagabundeo -= Time.deltaTime;
        if (Vector2.Distance(transform.position, _puntoDestinoVagabundeo) < 0.5f || _tiempoVagabundeo <= 0)
        {
            ElegirNuevoDestinoVagabundeo();
        }
        VolarHaciaDestino(_puntoDestinoVagabundeo);
    }

    private void ComportamientoAtaqueYPersecucion()
    {
        Vector2 direccionHaciaPlayer = ((Vector2)transform.position - (Vector2)_player.position).normalized;
        if (direccionHaciaPlayer.y < 0.2f) direccionHaciaPlayer.y = 0.5f; 
        direccionHaciaPlayer.Normalize();

        Vector2 puntoOptimo = (Vector2)_player.position + (direccionHaciaPlayer * distanciaOptimaAtaque);
        VolarHaciaDestino(puntoOptimo);

        _cronometroDisparo += Time.deltaTime;
        if (_cronometroDisparo >= cadenciaDisparo)
        {
            if (_animator != null) _animator.SetTrigger("Atacar"); 
            _cronometroDisparo = 0f;
        }
    }

    // Calcula el desplazamiento suavizado integrando un efecto de oscilación senoidal para el vuelo[cite: 13].
    private void VolarHaciaDestino(Vector2 destinoBase)
    {
        float offsetFloteY = Mathf.Sin(Time.time * velocidadFlote) * amplitudFlote;
        Vector2 destinoFinal = new Vector2(destinoBase.x, destinoBase.y + offsetFloteY);

        transform.position = Vector2.SmoothDamp(transform.position, destinoFinal, ref _velocidadSuavizada, suavidadMovimiento, velocidadVuelo);
    }

    private void ElegirNuevoDestinoVagabundeo()
    {
        _puntoDestinoVagabundeo = _posicionInicial + (Random.insideUnitCircle * radioVagabundeo);
        _tiempoVagabundeo = Random.Range(2f, 4f); 
    }

    // Ejecuta la reducción de salud, aplica fuerzas de retroceso e inicia los efectos visuales de daño[cite: 13].
    public void RecibirDano(int cantidad, Vector2 posicionAtacante)
    {
        if (_esInvulnerable) return;

        _vidaActual -= cantidad;
        Vector2 direccionGolpe = ((Vector2)transform.position - posicionAtacante).normalized;

        if (particulasGolpePrefab != null)
        {
            float angulo = Mathf.Atan2(direccionGolpe.y, direccionGolpe.x) * Mathf.Rad2Deg;
            Instantiate(particulasGolpePrefab, transform.position, Quaternion.Euler(0, 0, angulo));
        }
        
        if (_vidaActual <= 0)
        {
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.AddForce(direccionGolpe * fuerzaEmpuje, ForceMode2D.Impulse);
            }
            Morir();
        }
        else StartCoroutine(EfectoDanoRoutine(direccionGolpe));
    }

    // Gestiona la desactivación de físicas, la generación de recompensas y la destrucción tras los efectos sonoros[cite: 13].
    private void Morir()
    {
        if (_audioSource != null) 
        {
            _audioSource.Stop(); 
            if (sfxMorir != null) _audioSource.PlayOneShot(sfxMorir);
        }
        
        if (explosionMuertePrefab != null) Instantiate(explosionMuertePrefab, transform.position, Quaternion.identity);
        if (particulasGolpePrefab != null) Instantiate(particulasGolpePrefab, transform.position, Quaternion.identity);
        
        if (Random.Range(0f, 100f) <= probabilidadDropVida && prefabBolaVida != null)
        {
            Instantiate(prefabBolaVida, transform.position, Quaternion.identity);
        }

        if (prefabMonedaDorada != null)
        {
            int cantidadMonedas = Random.Range(minMonedasDrop, maxMonedasDrop + 1);
            for (int i = 0; i < cantidadMonedas; i++)
            {
                Instantiate(prefabMonedaDorada, transform.position, Quaternion.identity);
            }
        }

        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        if (_rb != null) _rb.simulated = false;

        float tiempoDestruccion = (sfxMorir != null) ? sfxMorir.length : 0.1f;
        Destroy(gameObject, tiempoDestruccion); 
    }

    private IEnumerator EfectoDanoRoutine(Vector2 direccionGolpe)
    {
        _esInvulnerable = true;
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero; 
            _rb.AddForce(direccionGolpe * fuerzaEmpuje, ForceMode2D.Impulse); 
        }

        if (materialFlashBlanco != null && _spriteRenderer != null)
        {
            _spriteRenderer.material = materialFlashBlanco;
            yield return new WaitForSeconds(0.15f); 
            _spriteRenderer.material = _materialOriginal;
        }
        else yield return new WaitForSeconds(0.15f);

        if (_rb != null) _rb.linearVelocity = Vector2.zero; 
        _velocidadSuavizada = Vector2.zero; 

        yield return new WaitForSeconds(0.1f); 
        _esInvulnerable = false;
    }

    private void VerificarDeteccionJugador()
    {
        float distanciaAlPlayer = Vector2.Distance(transform.position, _player.position);
        _jugadorDetectado = distanciaAlPlayer <= radioDeteccion;
    }

    // Instancia un proyectil dirigido hacia la posición actual del usuario y activa el audio de ataque[cite: 13].
    public void PerformShootEvent()
    {
        if (_audioSource != null && sfxAtacar != null) _audioSource.PlayOneShot(sfxAtacar);

        if (proyectilPrefab == null || puntoDisparo == null || _player == null) return;
        GameObject balaObj = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        if (balaObj.GetComponent<BalaEnemigo>() != null)
            balaObj.GetComponent<BalaEnemigo>().SetDireccion((_player.position - puntoDisparo.position).normalized);
    }
    private void GirarSpriteHaciaDestino()
    {
        if (_spriteRenderer == null) return;
        
        float direccionX = _velocidadSuavizada.x;

        if (direccionX > 0.1f)
        {
            _spriteRenderer.flipX = true; 
            if (puntoDisparo != null) puntoDisparo.localPosition = new Vector3(Mathf.Abs(_puntoDisparoOffsetBaseX), puntoDisparo.localPosition.y, puntoDisparo.localPosition.z);
        }
        else if (direccionX < -0.1f)
        {
            _spriteRenderer.flipX = false; 
            if (puntoDisparo != null) puntoDisparo.localPosition = new Vector3(-Mathf.Abs(_puntoDisparoOffsetBaseX), puntoDisparo.localPosition.y, puntoDisparo.localPosition.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        if (_player != null && _jugadorDetectado)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(_player.position, distanciaOptimaAtaque);
        }
        if (Application.isPlaying && !_jugadorDetectado)
        {
            Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(_posicionInicial, radioVagabundeo);
            Gizmos.DrawLine(transform.position, _puntoDestinoVagabundeo);
        }
    }
}