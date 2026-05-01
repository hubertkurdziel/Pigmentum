using UnityEngine;
using System.Collections;

public class FungusIA : MonoBehaviour
{
    [Header("Vida y Daño")]
    public int vidaMaxima = 3;
    private int _vidaActual;
    private bool _esInvulnerable = false;
    private bool _enKnockback = false; 
    
    public GameObject particulasGolpePrefab; 
    public GameObject explosionMuertePrefab; 
    
    private Shader _shaderOriginal; 

    [Header("Drops (Recompensas)")]
    [SerializeField] private GameObject prefabBolaVida;
    [SerializeField, Range(0f, 100f)] private float probabilidadDropVida = 20f;
    [SerializeField] private GameObject prefabMonedaDorada;
    [SerializeField] private int minMonedasDrop = 1;
    [SerializeField] private int maxMonedasDrop = 3;

    [Header("Knockback")]
    public float fuerzaKnockbackX = 5f;
    public float fuerzaKnockbackY = 3f;

    [Header("Movimiento (Saltitos)")]
    public float fuerzaSalto = 15f;
    public float velocidadHorizontal = 10f;
    public float tiempoEntreSaltos = 2f;
    public LayerMask capaSuelo;
    public Transform comprobadorSuelo;

    [Header("Ataque y Explosión")]
    public float radioDeteccion = 15f;   
    public float radioPersecucion = 12f;   
    public float distanciaParaExplotar = 2f; 
    public GameObject nubeToxicaPrefab; 

    [Header("Sonidos")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip sfxSalto;
    [SerializeField] private AudioClip sfxRecibirDano;
    [SerializeField] private AudioClip sfxMorir;
    [SerializeField] private AudioClip sfxCargarExplosion; 
    [SerializeField] private AudioClip sfxExplotar;        

    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
    private Transform _player;
    private bool _isGrounded;
    private bool _estaAtacando = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_spriteRenderer != null) _shaderOriginal = _spriteRenderer.material.shader;
        
        _vidaActual = vidaMaxima;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        StartCoroutine(RutinaIA());
    }

    void Update()
    {
        if (comprobadorSuelo == null) return; 

        _isGrounded = Physics2D.OverlapCircle(comprobadorSuelo.position, 0.2f, capaSuelo);
        if (_anim != null) _anim.SetBool("isGrounded", _isGrounded);

        if (_isGrounded && !_anim.GetBool("isWalking") && !_enKnockback)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        }
    }

    // Gestiona la reducción de salud y la aplicación de fuerzas de retroceso por impacto[cite: 15].
    public void RecibirDano(int cantidad, Vector2 posicionAtacante)
    {
        if (_esInvulnerable) return; 

        if (_audioSource != null && sfxRecibirDano != null && _vidaActual - cantidad > 0)
        {
            _audioSource.PlayOneShot(sfxRecibirDano);
        }

        _vidaActual -= cantidad;
        float direccionEmpuje = transform.position.x > posicionAtacante.x ? 1f : -1f;
        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        _rb.AddForce(new Vector2(direccionEmpuje * fuerzaKnockbackX, fuerzaKnockbackY), ForceMode2D.Impulse);

        if (particulasGolpePrefab != null) Instantiate(particulasGolpePrefab, transform.position, Quaternion.identity);

        if (_vidaActual <= 0) Morir();
        else StartCoroutine(EfectoDanoRoutine());
    }

    private IEnumerator EfectoDanoRoutine()
    {
        _esInvulnerable = true;
        _enKnockback = true;

        if (_spriteRenderer != null)
        {
            Shader shaderBlanco = Shader.Find("GUI/Text Shader");
            _spriteRenderer.material.shader = shaderBlanco;
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.12f); 
            _spriteRenderer.material.shader = _shaderOriginal;
        }

        yield return new WaitForSeconds(0.1f); 
        _esInvulnerable = false;
        _enKnockback = false;
    }

    private void Morir()
    {
        // Controla la desactivación de componentes físicos y visuales antes de la eliminación del objeto[cite: 15].
        if (_audioSource != null && sfxMorir != null)
        {
            _audioSource.PlayOneShot(sfxMorir);
        }

        if (explosionMuertePrefab != null) Instantiate(explosionMuertePrefab, transform.position, Quaternion.identity);

        if (Random.Range(0f, 100f) <= probabilidadDropVida && prefabBolaVida != null)
        {
            Instantiate(prefabBolaVida, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (prefabMonedaDorada != null)
        {
            int cantidadMonedas = Random.Range(minMonedasDrop, maxMonedasDrop + 1);
            for (int i = 0; i < cantidadMonedas; i++)
            {
                Instantiate(prefabMonedaDorada, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }

        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        if (_rb != null) _rb.simulated = false;

        float tiempoDestruccion = (sfxMorir != null) ? sfxMorir.length : 0.1f;
        Destroy(gameObject, tiempoDestruccion);
    }

    IEnumerator RutinaIA()
    {
        // Bucle de decisión que alterna entre persecución, ataque por proximidad o reposo[cite: 15].
        while (true)
        {
            if (!_estaAtacando && _player != null)
            {
                float distancia = Vector2.Distance(transform.position, _player.position);

                if (distancia < distanciaParaExplotar) yield return StartCoroutine(SecuenciaExplosion());
                else if (distancia < radioPersecucion && _isGrounded)
                {
                    yield return new WaitForSeconds(tiempoEntreSaltos);
                    if (!_estaAtacando && _isGrounded && !_enKnockback) SaltarHaciaPlayer();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void SaltarHaciaPlayer()
    {
        if (_player == null) return;

        if (_audioSource != null && sfxSalto != null)
        {
            _audioSource.PlayOneShot(sfxSalto);
        }

        float dirX = _player.position.x > transform.position.x ? 1f : -1f;
        transform.localScale = new Vector3(-dirX, 1, 1);
        _rb.AddForce(new Vector2(dirX * velocidadHorizontal, fuerzaSalto), ForceMode2D.Impulse);
        if (_anim != null) _anim.SetBool("isWalking", true);
        StartCoroutine(EsperarAterrizaje());
    }

    IEnumerator EsperarAterrizaje()
    {
        yield return new WaitForSeconds(0.2f); 
        while (!_isGrounded) yield return null;
        if (_anim != null) _anim.SetBool("isWalking", false);
    }

    IEnumerator SecuenciaExplosion()
    {
        // Gestiona la secuencia de carga, activación del ataque de área y fase de recuperación[cite: 15].
        _estaAtacando = true;

        if (_audioSource != null && sfxCargarExplosion != null)
        {
            _audioSource.PlayOneShot(sfxCargarExplosion);
        }

        if (_anim != null) _anim.SetBool("isWalking", false);
        _rb.linearVelocity = Vector2.zero; 
        if (_anim != null) _anim.SetTrigger("Explode"); 
        
        yield return new WaitForSeconds(1.2f); 
        if (_anim != null) _anim.SetBool("isRegenerating", true);
        yield return new WaitForSeconds(3.5f); 
        if (_anim != null) _anim.SetBool("isRegenerating", false);
        yield return new WaitForSeconds(1.0f); 

        _estaAtacando = false;
    }

    public void EjecutarExplosionFisica()
    {
        if (_audioSource != null && sfxExplotar != null)
        {
            _audioSource.PlayOneShot(sfxExplotar);
        }

        if (nubeToxicaPrefab != null) Instantiate(nubeToxicaPrefab, transform.position, Quaternion.identity);
    }
}