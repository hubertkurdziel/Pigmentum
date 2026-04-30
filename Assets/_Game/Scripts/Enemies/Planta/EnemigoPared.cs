using UnityEngine;
using System.Collections;

public class EnemigoPared : MonoBehaviour
{
    [Header("Ajustes")]
    public float rangoDeteccion = 10f;
    public float cadenciaDisparo = 2.5f;
    public Transform puntoDisparo;
    public GameObject proyectilPrefab;
    public GameObject explosionPrefab; 

    [Header("Drops (Recompensas)")]
    [SerializeField] private GameObject prefabBolaVida;
    [SerializeField, Range(0f, 100f)] private float probabilidadDropVida = 20f;
    [SerializeField] private GameObject prefabMonedaDorada;
    [SerializeField] private int minMonedasDrop = 1;
    [SerializeField] private int maxMonedasDrop = 3;

    [Header("Efectos Visuales")]
    public Material materialBlanco; 
    private Material _materialOriginal;

    [Header("Efectos de Sonido")]
    public AudioClip sfxEscupir; // <-- NUEVO: Sonido al disparar la bola

    [Header("Vida")]
    public int vidaMaxima = 3;
    private int _vidaActual;
    private bool _estaMuerto = false;

    private float _timerDisparo;
    private Transform _player;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
    private float _puntoDisparoOffsetBaseX;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
        _vidaActual = vidaMaxima;
        if (_spriteRenderer != null) _materialOriginal = _spriteRenderer.material;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;

        if (puntoDisparo != null) _puntoDisparoOffsetBaseX = puntoDisparo.localPosition.x;
    }

    private void Update()
    {
        if (_player == null || _estaMuerto) return;

        GirarSpriteHaciaJugador();

        float distancia = Vector2.Distance(transform.position, _player.position);
        
        if (distancia <= rangoDeteccion)
        {
            _timerDisparo += Time.deltaTime;
            if (_timerDisparo >= cadenciaDisparo)
            {
                LlamarAnimacionAtaque();
                _timerDisparo = 0f;
            }
        }
        else { _timerDisparo = 0f; }
    }

    private void LlamarAnimacionAtaque()
    {
        if (_anim != null) _anim.SetTrigger("Atacar"); 
    }

    public void RealizarDisparoInstancia()
    {
        if (_estaMuerto) return; 

        if (proyectilPrefab != null && puntoDisparo != null && _player != null)
        {
            // --- NUEVO: Reproducir sonido de escupir ---
            if (AudioManager.Instancia != null && sfxEscupir != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxEscupir);
            }

            Vector2 direccion = _player.position - puntoDisparo.position;
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            Quaternion rotacionInicial = Quaternion.Euler(0, 0, angulo);

            GameObject bola = Instantiate(proyectilPrefab, puntoDisparo.position, rotacionInicial);
            
            ProyectilPerseguidor scriptBola = bola.GetComponent<ProyectilPerseguidor>();
            if (scriptBola != null) scriptBola.Configurar(transform);
        }
    }

    private void GirarSpriteHaciaJugador()
    {
        if (_spriteRenderer == null) return;
        if (_player.position.x > transform.position.x)
        {
            _spriteRenderer.flipX = true; 
            if (puntoDisparo != null) puntoDisparo.localPosition = new Vector3(Mathf.Abs(_puntoDisparoOffsetBaseX), puntoDisparo.localPosition.y, puntoDisparo.localPosition.z);
        }
        else if (_player.position.x < transform.position.x)
        {
            _spriteRenderer.flipX = false; 
            if (puntoDisparo != null) puntoDisparo.localPosition = new Vector3(-Mathf.Abs(_puntoDisparoOffsetBaseX), puntoDisparo.localPosition.y, puntoDisparo.localPosition.z);
        }
    }

    public void RecibirDano(int cantidad, Vector2 posAtacante)
    {
        if (_estaMuerto) return;
        _vidaActual -= cantidad;
        StartCoroutine(EfectoParpadeo());
        if (_vidaActual <= 0) Morir();
    }

    private IEnumerator EfectoParpadeo()
    {
        if (_spriteRenderer != null && materialBlanco != null)
        {
            _spriteRenderer.material = materialBlanco; 
            yield return new WaitForSeconds(0.1f);
            if (_spriteRenderer != null) _spriteRenderer.material = _materialOriginal; 
        }
    }

    private void Morir()
    {
        _estaMuerto = true;
        if (explosionMuertePrefab() != null) Instantiate(explosionMuertePrefab(), transform.position, Quaternion.identity);

        // --- Drop de Vida ---
        if (Random.Range(0f, 100f) <= probabilidadDropVida && prefabBolaVida != null)
        {
            Instantiate(prefabBolaVida, transform.position, Quaternion.identity);
        }

        // --- Drop de Monedas (Efecto Piñata) ---
        if (prefabMonedaDorada != null)
        {
            int cantidadMonedas = Random.Range(minMonedasDrop, maxMonedasDrop + 1);
            for (int i = 0; i < cantidadMonedas; i++)
            {
                Instantiate(prefabMonedaDorada, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject); 
    }

    private GameObject explosionMuertePrefab() => explosionPrefab;
}