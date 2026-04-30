using System.Collections;
using UnityEngine;

public class BossFinal : MonoBehaviour
{
    private enum FaseBoss { Fase1_Suelo, Transicion, Fase2_Aire }
    private enum EstadoBoss { Esperando, Embestida, VolandoLibre, AtacandoAbanico, Culetazo }

    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMax = 20;
    private int _vidaActual;
    [SerializeField, Range(0f, 1f)] private float porcentajeFase2 = 0.5f;

    [Header("Drops (Recompensas)")]
    [SerializeField] private GameObject prefabBolaVida;
    [SerializeField, Range(0f, 100f)] private float probabilidadDropVida = 50f; 
    [SerializeField] private GameObject prefabMonedaDorada;
    [SerializeField] private int minMonedasDrop = 5;  
    [SerializeField] private int maxMonedasDrop = 12;

    [Header("IA y Tiempos")]
    private FaseBoss _faseActual = FaseBoss.Fase1_Suelo;
    private EstadoBoss _estadoActual = EstadoBoss.Esperando;
    [SerializeField] private float tiempoMinEspera = 2f;
    [SerializeField] private float tiempoMaxEspera = 4f;

    [Header("Fase 1: Embestida")]
    [SerializeField] private float tiempoAvisoEmbestida = 0.5f;
    [SerializeField] private float velocidadEmbestida = 18f;
    [SerializeField] private float tiempoRecuperacion = 1.2f;

    [Header("Fase 2: Vuelo")]
    [SerializeField] private float alturaVuelo = 6f;
    [SerializeField] private float velocidadVuelo = 6f;
    [SerializeField] private Transform puntoCentroEscenario; 

    [Header("Fase 2: Ataque Aleatorio")] 
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private int cantidadProyectiles = 15; 
    [SerializeField] private float cadenciaFuego = 0.1f; 
    [SerializeField] private float anguloApertura = 120f;
    [SerializeField] private float tiempoPreparacionAtaque = 1.2f;

    [Header("Fase 2: Ataque Culetazo")]
    [SerializeField] private float velocidadCaidaCuletazo = 25f;
    [SerializeField] private float tiempoAvisoCuletazo = 0.5f;
    [SerializeField] private float tiempoRecuperacionCuletazo = 1f;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sfxEmbestida;
    [SerializeField] private AudioClip sfxVolar;    
    [SerializeField] private AudioClip sfxEscupir;  
    [SerializeField] private AudioClip sfxAplastas; 
    [SerializeField] private AudioClip sfxChoquePared; 
    [SerializeField] private AudioClip sfxMuerte;
    [SerializeField] private AudioClip musicaBoss;

    [Header("Efectos Visuales")] 
    [SerializeField] private GameObject prefabExplosionMuerte;

    [Header("Sensores y Capas")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float distanciaPared = 0.5f;
    [SerializeField] private LayerMask capaEntorno; 

    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
    private Transform _jugador;
    private float _posYInicial;
    private int _direccionMirada = -1;
    private Shader _shaderOriginal;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _shaderOriginal = _spriteRenderer.material.shader;
        _vidaActual = vidaMax;
        _posYInicial = transform.position.y;
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) _jugador = obj.transform;
        
        if (AudioManager.Instancia != null && musicaBoss != null)
        {
            AudioManager.Instancia.ReproducirMusica(musicaBoss);
        }

        StartCoroutine(RutinaCerebro());
    }

    private IEnumerator RutinaCerebro()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (_estadoActual == EstadoBoss.Esperando && _jugador != null)
            {
                if (_faseActual == FaseBoss.Fase1_Suelo)
                {
                    ActualizarMirada();
                    yield return StartCoroutine(AtaqueEmbestida());
                }
                yield return new WaitForSeconds(Random.Range(tiempoMinEspera, tiempoMaxEspera));
            }
            yield return null;
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (_faseActual == FaseBoss.Transicion) return;
        _vidaActual -= cantidad;
        StartCoroutine(RutinaParpadeoDano());
        
        if (_faseActual == FaseBoss.Fase1_Suelo && _vidaActual <= vidaMax * porcentajeFase2)
        {
            StopAllCoroutines();
            StartCoroutine(SecuenciaTransicion());
        }
        
        if (_vidaActual <= 0) 
        {
            // --- NUEVO: Detenemos el sonido de volar (y cualquier otro SFX) antes de morir ---
            if (AudioManager.Instancia != null)
            {
                AudioManager.Instancia.DetenerSFX();
            }

            if (AudioManager.Instancia != null && sfxMuerte != null)
                AudioManager.Instancia.ReproducirSFX(sfxMuerte);

            if (prefabExplosionMuerte != null) 
                Instantiate(prefabExplosionMuerte, transform.position, Quaternion.identity);

            MenuFinDemo pantallaFinal = Object.FindFirstObjectByType<MenuFinDemo>();
            if (pantallaFinal != null)
            {
                pantallaFinal.IniciarSecuenciaFin(15f);
            }

            // Lógica de Drops (Vida y Monedas)[cite: 6]
            if (Random.Range(0f, 100f) <= probabilidadDropVida && prefabBolaVida != null)
            {
                GameObject vida = Instantiate(prefabBolaVida, transform.position + Vector3.up * 1f, Quaternion.identity);
                Rigidbody2D rbVida = vida.GetComponent<Rigidbody2D>();
                if (rbVida != null) rbVida.AddForce(new Vector2(Random.Range(-1f, 1f), 1f).normalized * 15f, ForceMode2D.Impulse);
            }

            if (prefabMonedaDorada != null)
            {
                int cantidadMonedas = Random.Range(minMonedasDrop, maxMonedasDrop + 1);
                for (int i = 0; i < cantidadMonedas; i++)
                {
                    GameObject moneda = Instantiate(prefabMonedaDorada, transform.position + Vector3.up * 1f, Quaternion.identity);
                    Rigidbody2D rbMoneda = moneda.GetComponent<Rigidbody2D>();
                    if (rbMoneda != null)
                    {
                        float dirX = Random.Range(-1f, 1f);
                        float dirY = Random.Range(0.2f, 1.5f); 
                        Vector2 direccionCaotica = new Vector2(dirX, dirY).normalized;
                        float fuerzaExplosiva = Random.Range(15f, 35f); 
                        rbMoneda.AddForce(direccionCaotica * fuerzaExplosiva, ForceMode2D.Impulse);
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator RutinaParpadeoDano()
    {
        Shader shaderBlanco = Shader.Find("GUI/Text Shader");
        _spriteRenderer.material.shader = shaderBlanco;
        _spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.material.shader = _shaderOriginal;
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.material.shader = shaderBlanco;
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.material.shader = _shaderOriginal;
    }
    
    private IEnumerator SecuenciaTransicion()
    {
        _faseActual = FaseBoss.Transicion;
        _rb.linearVelocity = Vector2.zero;
        _spriteRenderer.material.shader = _shaderOriginal;
        _spriteRenderer.color = Color.white;
        if (_anim != null) _anim.Play("Idle");
        _spriteRenderer.color = Color.red; 
        yield return new WaitForSeconds(1.5f);
        _spriteRenderer.color = Color.white;
        _faseActual = FaseBoss.Fase2_Aire;
        StartCoroutine(RutinaVueloLibre());
    }

    private IEnumerator AtaqueEmbestida()
    {
        _estadoActual = EstadoBoss.Embestida;
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(tiempoAvisoEmbestida);
        _spriteRenderer.color = Color.white;
        if (_anim != null) _anim.Play("Caminar");

        if (AudioManager.Instancia != null && sfxEmbestida != null)
            AudioManager.Instancia.ReproducirSFX(sfxEmbestida);

        float tiempoAtascado = 0f; 
        while (!TocandoPared())
        {
            _rb.linearVelocity = new Vector2(_direccionMirada * velocidadEmbestida, _rb.linearVelocity.y);
            if (Mathf.Abs(_rb.linearVelocity.x) < 1f)
            {
                tiempoAtascado += Time.deltaTime;
                if (tiempoAtascado > 0.2f) break; 
            }
            else tiempoAtascado = 0f; 
            yield return null;
        }

        if (AudioManager.Instancia != null && sfxChoquePared != null)
            AudioManager.Instancia.ReproducirSFX(sfxChoquePared);

        _rb.linearVelocity = Vector2.zero;
        if (_anim != null) _anim.Play("Idle");
        ActualizarMirada();
        yield return new WaitForSeconds(tiempoRecuperacion);
        _estadoActual = EstadoBoss.Esperando;
    }   

    private IEnumerator RutinaVueloLibre()
    {
        _estadoActual = EstadoBoss.VolandoLibre;
        _rb.gravityScale = 0f;
        if (_anim != null) _anim.Play("Volar");

        if (AudioManager.Instancia != null && sfxVolar != null)
            AudioManager.Instancia.ReproducirSFX(sfxVolar);
        
        float objetivoY = _posYInicial + alturaVuelo;
        while (transform.position.y < objetivoY)
        {
            _rb.linearVelocity = new Vector2(0f, velocidadVuelo);
            yield return null;
        }
        _rb.linearVelocity = Vector2.zero;
        
        while (true)
        {
            float probabilidad = Random.Range(0f, 100f);
            if (probabilidad <= 60f) yield return StartCoroutine(AtaqueCuletazo());
            else if (puntoCentroEscenario != null) yield return StartCoroutine(AtaqueAleatorio());
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator AtaqueAleatorio()
    {
        _estadoActual = EstadoBoss.AtacandoAbanico;
        ActualizarMirada();
        
        while (Vector2.Distance(transform.position, puntoCentroEscenario.position) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, puntoCentroEscenario.position, velocidadVuelo * Time.deltaTime);
            yield return null;
        }
        _rb.linearVelocity = Vector2.zero;

        Coroutine agitacionBoss = StartCoroutine(RutinaAgitarBoss(0.15f));
        if (_anim != null) _anim.SetTrigger("AtaqueAbanico");
        yield return new WaitForSeconds(tiempoPreparacionAtaque);

        if (prefabProyectil != null)
        {
            if (VibracionCamara.Instancia != null) 
                VibracionCamara.Instancia.EmpezarTerremoto(2f, 0.4f);

            float anguloCentral = -90f; 
            float anguloMin = anguloCentral - (anguloApertura / 2f);
            float anguloMax = anguloCentral + (anguloApertura / 2f);

            for (int i = 0; i < cantidadProyectiles; i++)
            {
                if (AudioManager.Instancia != null && sfxEscupir != null)
                    AudioManager.Instancia.ReproducirSFX(sfxEscupir);

                float anguloGiro = Random.Range(anguloMin, anguloMax);
                Quaternion rotacionProyectil = Quaternion.Euler(0, 0, anguloGiro - 90f);
                Instantiate(prefabProyectil, transform.position, rotacionProyectil);

                yield return new WaitForSeconds(Random.Range(cadenciaFuego * 0.5f, cadenciaFuego * 1.5f));
            }
        }

        if (agitacionBoss != null) StopCoroutine(agitacionBoss);
        transform.position = puntoCentroEscenario.position; 
        yield return new WaitForSeconds(1f);
        if (_anim != null) _anim.Play("Volar");
        _estadoActual = EstadoBoss.VolandoLibre;
    }

    private IEnumerator AtaqueCuletazo()
    {
        _estadoActual = EstadoBoss.Culetazo;
        _rb.linearVelocity = Vector2.zero;

        float tiempoPersecucion = 1.5f;
        while (tiempoPersecucion > 0 && _jugador != null)
        {
            tiempoPersecucion -= Time.deltaTime;
            Vector2 objetivoX = new Vector2(_jugador.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, objetivoX, velocidadVuelo * 1.5f * Time.deltaTime);
            ActualizarMirada();
            if (Mathf.Abs(transform.position.x - _jugador.position.x) < 0.2f) break;
            yield return null;
        }

        _spriteRenderer.color = Color.red;
        Coroutine agitacionBoss = StartCoroutine(RutinaAgitarBoss(0.1f));
        yield return new WaitForSeconds(tiempoAvisoCuletazo);
        if (agitacionBoss != null) StopCoroutine(agitacionBoss);
        _spriteRenderer.color = Color.white;

        while (transform.position.y > _posYInicial + 0.2f)
        {
            _rb.linearVelocity = new Vector2(0f, -velocidadCaidaCuletazo);
            yield return null;
        }

        _rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(transform.position.x, _posYInicial, transform.position.z);
        
        if (AudioManager.Instancia != null && sfxAplastas != null)
            AudioManager.Instancia.ReproducirSFX(sfxAplastas);

        if (VibracionCamara.Instancia != null) 
            VibracionCamara.Instancia.EmpezarTerremoto(0.4f, 1.2f);
            
        if (_anim != null) _anim.Play("Idle");
        yield return new WaitForSeconds(tiempoRecuperacionCuletazo);
        if (_anim != null) _anim.Play("Volar");

        if (AudioManager.Instancia != null && sfxVolar != null)
            AudioManager.Instancia.ReproducirSFX(sfxVolar);
        
        float objetivoY = _posYInicial + alturaVuelo;
        while (transform.position.y < objetivoY)
        {
            _rb.linearVelocity = new Vector2(0f, velocidadVuelo);
            yield return null;
        }
        _rb.linearVelocity = Vector2.zero;
        _estadoActual = EstadoBoss.VolandoLibre;
    }

    private IEnumerator RutinaAgitarBoss(float intensidad)
    {
        Vector3 posicionBase = transform.position;
        while (true)
        {
            transform.position = posicionBase + (Vector3)Random.insideUnitCircle * intensidad;
            yield return new WaitForSeconds(0.02f); 
        }
    }

    private void ActualizarMirada()
    {
        if (_jugador == null) return;
        _direccionMirada = _jugador.position.x > transform.position.x ? 1 : -1;
        Voltear(_direccionMirada);
    }

    private void Voltear(int dir)
    {
        if (dir == 1) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }

    private bool TocandoPared()
    {
        if (wallCheck == null) return false;
        return Physics2D.Raycast(wallCheck.position, Vector2.right * _direccionMirada, distanciaPared, capaEntorno);
    }
}