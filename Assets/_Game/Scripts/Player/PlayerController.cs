using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Tilemaps; 
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("GameFeel - Movimiento")]
    [SerializeField] private float moveSpeed = 9f;
    [SerializeField, Tooltip("Rapidez para alcanzar la velocidad máxima")] private float aceleracion = 12f;
    [SerializeField, Tooltip("Rapidez para frenar al soltar el mando")] private float deceleracion = 16f;
    [SerializeField, Range(0f, 1f), Tooltip("Control en el aire (1 = total, 0 = nada)")] private float multiplicadorControlAire = 0.8f;

    [Header("GameFeel - Salto y Gravedad")]
    [SerializeField] private float jumpForce = 22f; 
    [SerializeField, Tooltip("Gravedad normal del mundo")] private float gravedadBase = 4f;
    [SerializeField, Tooltip("Multiplicador de gravedad al caer (hace que caiga más rápido y pesado)")] private float multiplicadorCaida = 1.8f;
    [SerializeField, Tooltip("Límite máximo de velocidad al caer")] private float velocidadMaximaCaida = 25f;
    [SerializeField, Range(0f, 1f), Tooltip("Cuánto se frena el salto si sueltas el botón en el aire")] private float multiplicadorCorteSalto = 0.5f;

    [Header("GameFeel - Asistencias (Perdón)")]
    [SerializeField, Tooltip("Tiempo para saltar tras caer de un borde")] private float coyoteTime = 0.15f;
    [SerializeField, Tooltip("Tiempo que recuerda el botón de salto antes de tocar el suelo")] private float jumpBufferTime = 0.15f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 24f; 
    [SerializeField] private float dashDuration = 0.15f; 
    [SerializeField] private float dashCooldown = 1f; 
    private bool _isDashing; 
    private bool _canDash = true; 

    [Header("Escalada y Estamina")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float slideSpeed = 3f;
    [SerializeField] private float maxStamina = 3f;
    [SerializeField] private float costeEstaminaDash = 1f; 
    [SerializeField] private float velocidadRecuperacionEstamina = 2.5f; 
    [SerializeField] private float ledgeHangXOffset = 0f; 
    [SerializeField] private float ledgeHangYOffset = 1.4f;
    [SerializeField] private float wallClimbXOffset = 0.2f; 
    [SerializeField] private float ledgeClimbXOffset = 1.0f;
    [SerializeField] private float ledgeClimbYOffset = 2.5f;

    [Header("UI Estamina (Zelda Style)")]
    [SerializeField] private Canvas canvasEstamina; 
    [SerializeField] private GameObject contenedorEstaminaUI; 
    [SerializeField] private Image circuloEstaminaUI; 
    
    [SerializeField] private float tiempoParaOcultar = 1.5f; 
    [SerializeField] private float velocidadSeguimientoUI = 20f; 
    [SerializeField] private Vector3 offsetUI = new Vector3(-0.6f, 1.2f, 0f); 
    private float _temporizadorOcultarUI = 0f;

    [Header("Combate")]
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private float radioAtaque = 0.8f;
    [SerializeField] private LayerMask capaEnemigos;
    [SerializeField] private int danoAtaque = 1;
    [SerializeField] private float tiempoEntreAtaques = 0.4f;
    [SerializeField] private float fuerzaRebotePogo = 20f; 
    private float _siguienteAtaqueTiempo = 0f;

    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMax = 5; 
    [SerializeField] private int vidaActual;
    [SerializeField] private GameObject contenedorVida; 
    [SerializeField] private GameObject prefabGotaVida; 
    [SerializeField] private float tiempoInvulnerabilidad = 1.5f;
    private bool _esInvulnerable = false;

    [Header("Knockback (Empuje por daño)")]
    [SerializeField] private float knockbackForceX = 12f; 
    [SerializeField] private float knockbackForceY = 6f;  
    [SerializeField] private float knockbackDuration = 0.5f; 
    [SerializeField] private Sprite hurtSprite; 
    private bool _isKnockedBack = false;

    [Header("Gancho (Grapple)")]
    [SerializeField] private float radioGancho = 8f; 
    [SerializeField] private float fuerzaBalanceo = 10f; 
    [SerializeField] private LayerMask grappleLayer; 

    [Header("Sensores")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private Vector2 wallCheckSize   = new Vector2(0.2f, 0.4f);
    
    [Header("Capas (Tilemaps)")]
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem polvoPasos;
    [SerializeField] private GameObject transicionMuertePrefab; 
    [SerializeField] private GameObject orbesMuertePrefab;

    [Header("Sonidos de Pasos por Material (Tilemap)")]
    [SerializeField] private AudioClip[] sfxPasosPorDefecto; 
    [SerializeField] private AudioClip[] sfxPasosHierba;
    [SerializeField] private AudioClip[] sfxPasosMadera;
    [SerializeField] private AudioClip[] sfxPasosPiedra;
    [SerializeField] private float tiempoEntrePasos = 0.3f;
    [SerializeField] private AudioSource altavozPasos; 

    private float _pasoTimer;
    private string _nombreTileActual = ""; 

    [Header("Sonidos de Combate y Vida")]
    [SerializeField] private AudioClip sfxAtaque;         
    [SerializeField] private AudioClip sfxImpactoEnemigo; 
    [SerializeField] private AudioClip sfxRecibirDano;    
    [SerializeField] private AudioClip sfxRecuperarVida;

    [Header("Sonidos de Acción")]
    [SerializeField] private AudioClip sfxSaltoCorto; 
    [SerializeField] private AudioClip sfxSaltoLargo; 
    [SerializeField] private AudioClip sfxDash;
    [SerializeField] private AudioClip sfxGancho;
    [SerializeField] private AudioClip sfxBalanceoBucle;
    
    [Header("Sonidos de Escalada")]
    [SerializeField] private AudioClip sfxAgarreBorde;
    [SerializeField] private AudioClip sfxEscalando;
    [SerializeField] private AudioClip sfxDeslizar;


    [Header("Efectos Visuales - Dash (Estela)")]
    [SerializeField] private float tiempoEntreFantasmas = 0.05f; 
    [SerializeField] private float duracionFantasma = 0.3f; 
    [SerializeField] private Color colorEstela = new Color(0f, 0.5f, 1f, 0.5f); 

    [Header("Coleccionables")]
    public int totalGotas = 0;
    public TMPro.TextMeshProUGUI textoGotasUI; 

    // ── Referencias ──────────────────────────────────────────────
    private Rigidbody2D         _rb;
    private Collider2D          _col; 
    private InputSystem_Actions _controls;
    private Animator            _anim;
    private SpriteRenderer      _spriteRenderer;
    private DistanceJoint2D     _dj; 
    private LineRenderer        _lr; 

    // ── Input & Timers ───────────────────────────────────────────
    private Vector2 _moveInput;
    private bool    _isHoldingGrab;
    private bool    _dropPressed;
    private float   _coyoteTimeCounter;
    private float   _jumpBufferCounter;

    // ── Sensores & Estado ─────────────────────────────────────────
    private bool _isGrounded;
    private bool _isTouchingWall;
    private bool _isTouchingWallAbove;

    private enum ClimbState { None, Climbing, AtLedge, Exhausted, Swinging }
    private ClimbState _climbState = ClimbState.None;

    // ── Misc ─────────────────────────────────────────────────────
    private float   _currentStamina;
    private int     _facingDirection = 1;
    private bool    _isLedgeRoutineRunning = false;
    private float   _wallJumpCooldown = 0f;
    private float _puntoAtaqueOffsetBaseX;
    private float _puntoAtaqueOffsetBaseY; 
    
    private float _wallCheckOffsetBaseX;
    private float _ledgeCheckOffsetBaseX;

    private Vector2 _posicionInicial;
    private bool _estaReapareciendo = false;

    private void Awake()
    {
        _rb             = GetComponent<Rigidbody2D>();
        _col            = GetComponent<Collider2D>();
        _anim           = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _controls       = new InputSystem_Actions();
        
        _dj = GetComponent<DistanceJoint2D>();
        if (_dj != null) _dj.enabled = false;

        _lr = GetComponent<LineRenderer>();
        if (_lr != null) _lr.enabled = false;

        _controls.Player.Jump.performed += _ => OnJumpInput();
        _controls.Player.Jump.canceled += _ => OnJumpCanceled(); 

        _currentStamina = maxStamina;
        vidaActual = vidaMax; 
        _posicionInicial = transform.position;

        if (puntoAtaque != null) 
        {
            _puntoAtaqueOffsetBaseX = puntoAtaque.localPosition.x;
            _puntoAtaqueOffsetBaseY = puntoAtaque.localPosition.y;
        }

        if (wallCheck != null) _wallCheckOffsetBaseX = wallCheck.localPosition.x;
        if (ledgeCheck != null) _ledgeCheckOffsetBaseX = ledgeCheck.localPosition.x;
    }

    private void Start()
    {
        if (GameManager.Instancia != null && GameManager.Instancia.ultimoHardCheckpoint == Vector2.zero)
        {
            GameManager.Instancia.ultimoHardCheckpoint = transform.position;
            GameManager.Instancia.ultimoSoftCheckpoint = transform.position;
        }
    }

    private void OnEnable()
    {
        if (_controls == null) _controls = new InputSystem_Actions();
        _controls.Enable();
    }

    private void OnDisable() => _controls?.Disable();

    private void Update()
    {
        if (_isLedgeRoutineRunning || _isKnockedBack || _isDashing || _estaReapareciendo) return; 
        if (_controls == null) return;

        if (Time.timeScale == 0f) return;

        if (DialogueManager.Instancia != null && DialogueManager.Instancia.DialogoActivo())
        {
            _moveInput = Vector2.zero; 
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y); 
            UpdateAnimations(); 
            return; 
        }

        CheckSurroundings();

        if (_isGrounded) _coyoteTimeCounter = coyoteTime;
        else _coyoteTimeCounter -= Time.deltaTime;

        _jumpBufferCounter -= Time.deltaTime;

        HandleJumpExecution(); 

        bool usandoEstamina = _climbState == ClimbState.Climbing || _climbState == ClimbState.AtLedge;

        if (_isGrounded && _climbState == ClimbState.None)
        {
            if (_currentStamina < maxStamina)
            {
                _currentStamina += velocidadRecuperacionEstamina * Time.deltaTime;
                if (_currentStamina > maxStamina) _currentStamina = maxStamina;
            }
        }

        if (usandoEstamina || _currentStamina < maxStamina) _temporizadorOcultarUI = tiempoParaOcultar;
        else if (_temporizadorOcultarUI > 0) _temporizadorOcultarUI -= Time.deltaTime;

        if (canvasEstamina != null && contenedorEstaminaUI != null && circuloEstaminaUI != null)
        {
            contenedorEstaminaUI.SetActive(_temporizadorOcultarUI > 0);
            circuloEstaminaUI.fillAmount = _currentStamina / maxStamina;
        }

        if (_wallJumpCooldown > 0f) _wallJumpCooldown -= Time.deltaTime;

        _moveInput = _controls.Player.Move.ReadValue<Vector2>();

        if (_climbState == ClimbState.None)
        {
            if (_moveInput.x > 0) _facingDirection =  1;
            else if (_moveInput.x < 0) _facingDirection = -1;
        }

        // --- 1. ESCALAR Y SOLTARSE ---
        _isHoldingGrab = false;
        _dropPressed = false;

        // Teclado
        if (Keyboard.current != null) 
        {
            _isHoldingGrab |= Keyboard.current.leftShiftKey.isPressed;
            _dropPressed |= Keyboard.current.qKey.wasPressedThisFrame;
        }
        // Mando: Escalar en L2 (leftTrigger), Soltarse en Círculo (buttonEast)
        if (Gamepad.current != null) 
        {
            _isHoldingGrab |= Gamepad.current.leftTrigger.isPressed; 
            _dropPressed |= Gamepad.current.buttonEast.wasPressedThisFrame; 
        }

        // --- 2. PEGAR / ATACAR ---
        bool pulsadoAtaque = false;
        if (Mouse.current != null) pulsadoAtaque |= Mouse.current.leftButton.wasPressedThisFrame;
        if (Keyboard.current != null) pulsadoAtaque |= Keyboard.current.jKey.wasPressedThisFrame;
        // Mando: Pegar en Cuadrado (buttonWest)
        if (Gamepad.current != null) pulsadoAtaque |= Gamepad.current.buttonWest.wasPressedThisFrame;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            pulsadoAtaque = false; // Escudo del menú
        }

        if (pulsadoAtaque && Time.time >= _siguienteAtaqueTiempo && _climbState == ClimbState.None)
        {
            Atacar();
            _siguienteAtaqueTiempo = Time.time + tiempoEntreAtaques;
        }

        // --- 3. GANCHO ---
        bool botonGanchoPulsado = false;
        bool botonGanchoSoltado = false;
        if (Keyboard.current != null)
        {
            botonGanchoPulsado |= Keyboard.current.eKey.wasPressedThisFrame;
            botonGanchoSoltado |= Keyboard.current.eKey.wasReleasedThisFrame;
        }
        // Mando: Gancho en R1 (rightShoulder)
        if (Gamepad.current != null)
        {
            botonGanchoPulsado |= Gamepad.current.rightShoulder.wasPressedThisFrame;
            botonGanchoSoltado |= Gamepad.current.rightShoulder.wasReleasedThisFrame;
        }

        if (botonGanchoPulsado && _climbState == ClimbState.None) IntentarEnganchar();
        if (botonGanchoSoltado && _climbState == ClimbState.Swinging) SoltarGancho();

        // --- 4. DASH ---
        bool dashPulsado = false;
        if (Keyboard.current != null) dashPulsado |= Keyboard.current.leftShiftKey.wasPressedThisFrame;
        // Mando: Dash en R2 (rightTrigger)
        if (Gamepad.current != null) dashPulsado |= Gamepad.current.rightTrigger.wasPressedThisFrame;
        
        if (dashPulsado && _canDash && _climbState == ClimbState.None && _currentStamina >= costeEstaminaDash)
        {
            _currentStamina -= costeEstaminaDash; 
            _temporizadorOcultarUI = tiempoParaOcultar; 
            
            Vector2 direccionDash = _moveInput.normalized;
            if (direccionDash == Vector2.zero) direccionDash = new Vector2(_facingDirection, 0f);
            
            StartCoroutine(RutinaDash(direccionDash));
        }

        HandleClimbStateMachine();
        
        if (_climbState == ClimbState.Swinging && _lr != null && _dj != null)
        {
            _lr.SetPosition(0, transform.position); 
            _lr.SetPosition(1, _dj.connectedAnchor); 
        }

        if (_isLedgeRoutineRunning) return; 

        UpdateAnimations();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (_isLedgeRoutineRunning || _isKnockedBack || _isDashing || _estaReapareciendo) return; 

        switch (_climbState)
        {
            case ClimbState.None: Move(); ApplyGravityModifiers(); break;
            case ClimbState.Climbing: ClimbPhysics(); break;
            case ClimbState.AtLedge: LedgeHangPhysics(); break;
            case ClimbState.Exhausted: ExhaustedPhysics(); break;
            case ClimbState.Swinging: SwingingPhysics(); break; 
        }
    }

    private void LateUpdate()
    {
        if (canvasEstamina != null && contenedorEstaminaUI != null && contenedorEstaminaUI.activeSelf)
        {
            float offsetXDinamico = _spriteRenderer.flipX ? Mathf.Abs(offsetUI.x) : -Mathf.Abs(offsetUI.x);
            Vector3 posicionObjetivo = transform.position + new Vector3(offsetXDinamico, offsetUI.y, offsetUI.z);
            canvasEstamina.transform.position = Vector3.Lerp(canvasEstamina.transform.position, posicionObjetivo, velocidadSeguimientoUI * Time.deltaTime);
        }
    }

    private bool _isHoldingJump = false; 

    private void OnJumpInput()
    {
        _jumpBufferCounter = jumpBufferTime;
        _isHoldingJump = true; 
    }

    private void OnJumpCanceled()
    {
        _isHoldingJump = false; 

        if (_rb.linearVelocity.y > 0 && !_isGrounded && _climbState == ClimbState.None)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * multiplicadorCorteSalto);
        }
    }
    private void HandleJumpExecution()
    {
        if (_jumpBufferCounter > 0f && !_isLedgeRoutineRunning && _wallJumpCooldown <= 0f && !_isKnockedBack)
        {
            if (_coyoteTimeCounter > 0f && _climbState == ClimbState.None)
            {
                _jumpBufferCounter = 0f;
                _coyoteTimeCounter = 0f;

                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f); 
                _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                if (_anim != null) _anim.SetTrigger("Jump");

                if (AudioManager.Instancia != null && sfxSaltoCorto != null) AudioManager.Instancia.ReproducirSFX(sfxSaltoCorto);
                StartCoroutine(ChequearSaltoLargo());
            }
            else if (_climbState == ClimbState.Climbing || _climbState == ClimbState.AtLedge || _climbState == ClimbState.Exhausted)
            {
                _jumpBufferCounter = 0f;
                ExitClimb();
                _rb.linearVelocity = Vector2.zero;
                _rb.AddForce(new Vector2(-_facingDirection * 10f, 25f), ForceMode2D.Impulse);
                _wallJumpCooldown  = 0.3f;
                
                if (_anim != null) _anim.Play("Jump_Spin", -1, 0f);

                if (AudioManager.Instancia != null && sfxSaltoCorto != null) AudioManager.Instancia.ReproducirSFX(sfxSaltoCorto);
                StartCoroutine(ChequearSaltoLargo());
            }
        }
    }

    private IEnumerator ChequearSaltoLargo()
    {
        yield return new WaitForSeconds(0.15f);

        if (_isHoldingJump && _rb.linearVelocity.y > 0)
        {
            if (AudioManager.Instancia != null && sfxSaltoLargo != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxSaltoLargo);
            }
        }
    }

    private void Move()
    {
        float targetSpeed = _moveInput.x * moveSpeed;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? aceleracion : deceleracion;

        if (!_isGrounded)
        {
            accelRate *= multiplicadorControlAire;
        }

        float newVelocityX = Mathf.MoveTowards(_rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime * 50f);
        _rb.linearVelocity = new Vector2(newVelocityX, _rb.linearVelocity.y);
        
        bool seEstaMoviendo = _isGrounded && Mathf.Abs(_moveInput.x) > 0.1f && _climbState == ClimbState.None;

        if (polvoPasos != null)
        {
            var em = polvoPasos.emission;
            em.enabled = seEstaMoviendo;
        }

        if (seEstaMoviendo)
        {
            _pasoTimer -= Time.deltaTime; 
            if (_pasoTimer <= 0f)
            {
                ReproducirPaso();
                _pasoTimer = tiempoEntrePasos; 
            }
        }
        else
        {
            _pasoTimer = 0f; 
            if (altavozPasos != null && altavozPasos.isPlaying) 
            {
                altavozPasos.Stop(); 
            }
        }
    }

    private void ApplyGravityModifiers()
    {
        if (_climbState != ClimbState.None) return; 

        if (_rb.linearVelocity.y < 0)
        {
            _rb.gravityScale = gravedadBase * multiplicadorCaida;
        }
        else
        {
            _rb.gravityScale = gravedadBase;
        }

        if (_rb.linearVelocity.y < -velocidadMaximaCaida)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -velocidadMaximaCaida);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo")) TomarDano(1, collision.contacts[0].point);
    }

    public void TomarDano(int cantidad, Vector2 pointOfContact)
    {
        if (_esInvulnerable || _isLedgeRoutineRunning || _isKnockedBack || _estaReapareciendo) return;

        if (AudioManager.Instancia != null && sfxRecibirDano != null) AudioManager.Instancia.ReproducirSFX(sfxRecibirDano);

        for (int i = 0; i < cantidad; i++)
        {
            if (vidaActual > 0)
            {
                vidaActual--; 
                if (contenedorVida != null && contenedorVida.transform.childCount > vidaActual)
                {
                    Transform gota = contenedorVida.transform.GetChild(vidaActual);
                    Animator animGota = gota.GetComponent<Animator>();
                    if (animGota != null) animGota.SetTrigger("Explotar"); 
                    Destroy(gota.gameObject, 0.5f); 
                }
            }
        }

        if (vidaActual > 0)
        {
            StartCoroutine(RutinaKnockback(pointOfContact)); 
            StartCoroutine(RutinaInvulnerabilidad()); 
        }
        else StartCoroutine(SecuenciaMuerte());
    }

    public void MuertePorTrampa(int cantidad)
    {
        if (_esInvulnerable || _isLedgeRoutineRunning || _estaReapareciendo) return;

        for (int i = 0; i < cantidad; i++)
        {
            if (vidaActual > 0)
            {
                vidaActual--; 
                if (contenedorVida != null && contenedorVida.transform.childCount > vidaActual)
                {
                    Transform gota = contenedorVida.transform.GetChild(vidaActual);
                    Animator animGota = gota.GetComponent<Animator>();
                    if (animGota != null) animGota.SetTrigger("Explotar"); 
                    Destroy(gota.gameObject, 0.5f); 
                }
            }
        }
        StartCoroutine(SecuenciaMuerte());
    }

    private IEnumerator RutinaKnockback(Vector2 pointOfContact)
    {
        _isKnockedBack = true;
        if (_anim != null) _anim.enabled = false; 
        if (hurtSprite != null) _spriteRenderer.sprite = hurtSprite; 

        float signX = Mathf.Sign(transform.position.x - pointOfContact.x);
        _rb.linearVelocity = new Vector2(signX * knockbackForceX, knockbackForceY);

        yield return new WaitForSeconds(knockbackDuration);

        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y); 
        if (_anim != null) _anim.enabled = true; 
        _isKnockedBack = false;
    }

    private IEnumerator RutinaInvulnerabilidad()
    {
        _esInvulnerable = true;

        int capaPlayer = LayerMask.NameToLayer("Player");
        int capaEnemigo = LayerMask.NameToLayer("Enemigos");
        
        if (capaPlayer != -1 && capaEnemigo != -1)
        {
            Physics2D.IgnoreLayerCollision(capaPlayer, capaEnemigo, true);
        }

        float tiempoPasado = 0;
        while (tiempoPasado < tiempoInvulnerabilidad)
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled; 
            yield return new WaitForSeconds(0.1f);
            tiempoPasado += 0.1f;
        }

        _spriteRenderer.enabled = true; 
        _esInvulnerable = false;

        if (capaPlayer != -1 && capaEnemigo != -1)
        {
            Physics2D.IgnoreLayerCollision(capaPlayer, capaEnemigo, false);
        }
    }

    private IEnumerator SecuenciaMuerte()
    {
        _estaReapareciendo = true;
        _spriteRenderer.enabled = false;
        if (_col != null) _col.enabled = false; 
        _rb.linearVelocity = Vector2.zero; 
        _rb.gravityScale = 0f; 
        _isKnockedBack = true; 

        if (orbesMuertePrefab != null) Instantiate(orbesMuertePrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.8f);

        if (transicionMuertePrefab != null) Instantiate(transicionMuertePrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        if (GameManager.Instancia != null)
        {
            if (vidaActual <= 0)
            {
                transform.position = GameManager.Instancia.ultimoHardCheckpoint;
                vidaActual = vidaMax; 
                RestaurarVidasUI(); 
                
                if (AudioManager.Instancia != null && sfxRecuperarVida != null) AudioManager.Instancia.ReproducirSFX(sfxRecuperarVida);
            }
            else transform.position = GameManager.Instancia.ultimoSoftCheckpoint;
        }
        else
        {
            transform.position = _posicionInicial;
            if (vidaActual <= 0) 
            {
                vidaActual = vidaMax;
                RestaurarVidasUI(); 
                
                if (AudioManager.Instancia != null && sfxRecuperarVida != null) AudioManager.Instancia.ReproducirSFX(sfxRecuperarVida);
            }
        }
        
        _spriteRenderer.enabled = true;
        if (_col != null) _col.enabled = true;
        _rb.gravityScale = gravedadBase;
        _isKnockedBack = false;
        _estaReapareciendo = false; 
        _currentStamina = maxStamina; 

        StartCoroutine(RutinaInvulnerabilidad());
    }

    private void HandleClimbStateMachine()
    {
        switch (_climbState)
        {
            case ClimbState.None:
                if (_isTouchingWall && _isHoldingGrab && _currentStamina > 0f && _wallJumpCooldown <= 0f) EnterClimb();
                break;
            case ClimbState.Climbing:
                if (!_isTouchingWallAbove && _moveInput.y > 0) { EnterLedge(); break; }
                if (_dropPressed) { EnterExhausted(); break; } // <-- CAMBIO AQUÍ
                if (!_isTouchingWall) { ExitClimb(); break; }
                if (_currentStamina <= 0f) EnterExhausted();   // <-- CAMBIO AQUÍ
                break;
            case ClimbState.AtLedge:
                if (_dropPressed) { EnterExhausted(); break; } // <-- CAMBIO AQUÍ
                
                bool subirBordePulsado = false;
                if (Keyboard.current != null) subirBordePulsado |= Keyboard.current.lKey.wasPressedThisFrame;
                if (Gamepad.current != null) subirBordePulsado |= Gamepad.current.leftShoulder.wasPressedThisFrame;

                if (subirBordePulsado) StartCoroutine(LedgeClimbRoutine());
                break;
            case ClimbState.Exhausted:
                if (_isGrounded) { ExitClimb(); break; }       // <-- CAMBIO AQUÍ
                if (!_isTouchingWall) ExitClimb();
                break;
        }
    }
    
    private void EnterClimb()
    {
        _climbState = ClimbState.Climbing;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero; 
        Vector2 rayStart = transform.position;
        Vector2 direccion = Vector2.right * _facingDirection;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direccion, 2f, wallLayer | floorLayer);
        if (hit.collider != null)
        {
            float perfectX = hit.point.x - (wallClimbXOffset * _facingDirection);
            transform.position = new Vector3(perfectX, transform.position.y, transform.position.z);
        }
    }

    private void EnterLedge()   
    {
        _climbState = ClimbState.AtLedge;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero;

        // 1. CORTAR SONIDO DE ESCALADA AL ENGANCHARSE AL BORDE
        if (altavozPasos != null) 
        { 
            altavozPasos.Stop(); 
            altavozPasos.loop = false; 
        }

        // 2. REPRODUCIR EL GOLPE DE AGARRE AL BORDE
        if (AudioManager.Instancia != null && sfxAgarreBorde != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxAgarreBorde);
        }

        // 3. Físicas del borde
        if (ledgeCheck != null && wallCheck != null)
        {
            Vector2 rayStart = new Vector2(wallCheck.position.x + (_facingDirection * 0.5f), ledgeCheck.position.y + 1f);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 3f, floorLayer | wallLayer);
            if (hit.collider != null)
            {
                float perfectY = hit.point.y - ledgeHangYOffset;
                float perfectX = transform.position.x + (ledgeHangXOffset * _facingDirection);
                transform.position = new Vector3(perfectX, perfectY, transform.position.z);
            }
        }
    }
    private void ExitClimb()
    {
        _climbState = ClimbState.None;
        _rb.gravityScale = gravedadBase;
        
        // CORTAR SONIDO AL SALTAR DE LA PARED O SOLTARSE
        if (altavozPasos != null) 
        { 
            altavozPasos.Stop(); 
            altavozPasos.loop = false; 
        }
    }
    private void EnterExhausted()
    {
        _climbState = ClimbState.Exhausted;
        
        // AL CAER/RESBALAR: Ponemos el sonido de resbalar, o cortamos si no hay
        if (altavozPasos != null)
        {
            if (sfxDeslizar != null)
            {
                altavozPasos.clip = sfxDeslizar;
                altavozPasos.loop = true;
                altavozPasos.Play();
            }
            else
            {
                altavozPasos.Stop();
                altavozPasos.loop = false;
            }
        }
    }

    private void ClimbPhysics()
    {
        _rb.gravityScale = 0f;
        _currentStamina -= Time.deltaTime; 
        float targetVelocityY = 0f;
        if (_moveInput.y > 0) targetVelocityY = climbSpeed;
        else if (_moveInput.y < 0) targetVelocityY = -climbSpeed;
        _rb.linearVelocity = new Vector2(0f, targetVelocityY);

        // --- LÓGICA DE SONIDO: ESCALADA EN BUCLE ---
        if (Mathf.Abs(targetVelocityY) > 0f)
        {
            if (altavozPasos != null && sfxEscalando != null)
            {
                // Solo le damos a Play si NO estaba sonando ya (evita el tartamudeo)
                if (altavozPasos.clip != sfxEscalando || !altavozPasos.isPlaying)
                {
                    altavozPasos.clip = sfxEscalando;
                    altavozPasos.loop = true;
                    altavozPasos.Play();
                }
            }
        }
        else
        {
            // Si el jugador se queda colgado y quieto, pausamos el sonido del roce
            if (altavozPasos != null && altavozPasos.clip == sfxEscalando)
            {
                altavozPasos.Pause(); 
            }
        }
    }

    private void LedgeHangPhysics()
    {
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero; 
    }

    private void ExhaustedPhysics()
    {
        _rb.gravityScale = 0f;
        _rb.linearVelocity = new Vector2(0f, -slideSpeed);
    }

    private IEnumerator LedgeClimbRoutine()
    {
        _isLedgeRoutineRunning = true;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero; 
        if (_anim != null) 
        {
            _anim.SetBool("isLedgeHanging", false);
            _anim.Play("ClimbUp", -1, 0f); 
        }
        yield return new WaitForSeconds(1f); 
        transform.position += new Vector3(_facingDirection * ledgeClimbXOffset, ledgeClimbYOffset, 0f); 
        _climbState = ClimbState.None;
        _rb.gravityScale = gravedadBase;
        _isLedgeRoutineRunning = false;
    }

    private void CheckSurroundings()
    {
        Collider2D sueloDetectado = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, floorLayer | wallLayer);
        _isGrounded = sueloDetectado != null;

        if (_isGrounded)
        {
            Tilemap tilemap = sueloDetectado.GetComponent<Tilemap>();
            
            if (tilemap != null)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(groundCheck.position - new Vector3(0f, 0.1f, 0f));
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    _nombreTileActual = tile.name.ToLower(); 
                    Debug.Log("Estoy pisando el bloque: " + _nombreTileActual);
                }
            }
            else
            {
                _nombreTileActual = sueloDetectado.tag.ToLower(); 
            }
        }

        _isTouchingWall  = Physics2D.OverlapBox(wallCheck.position,   wallCheckSize,   0f, wallLayer | floorLayer);
        _isTouchingWallAbove = ledgeCheck != null ? Physics2D.OverlapBox(ledgeCheck.position, wallCheckSize, 0f, wallLayer | floorLayer) : _isTouchingWall;
    }
    private void UpdateAnimations()
    {
        if (_anim == null || _isKnockedBack) return; 

        _anim.SetFloat("VerticalVelocity", _rb.linearVelocity.y);
        _anim.SetFloat("Speed", Mathf.Abs(_moveInput.x));
        
        bool animGrounded = _isGrounded && _climbState == ClimbState.None;
        _anim.SetBool("isGrounded", animGrounded);
        _anim.SetBool("isClimbing", _climbState == ClimbState.Climbing);
        _anim.SetBool("isLedgeHanging", _climbState == ClimbState.AtLedge); 
        _anim.SetBool("isSliding", _climbState == ClimbState.Exhausted);

        if (_climbState == ClimbState.Climbing) 
            _anim.speed = _anim.IsInTransition(0) ? 1f : (_moveInput.y > 0 ? 1f : 0f);
        else 
            _anim.speed = 1f; 
    }

    private void FlipSprite()
    {
        if (_climbState == ClimbState.Climbing || _climbState == ClimbState.AtLedge || _climbState == ClimbState.Exhausted || _isKnockedBack) return;

        bool atacandoVertical = (!_isGrounded && _moveInput.y < -0.3f) || (_moveInput.y > 0.3f); 

        if (_moveInput.x > 0) 
        {
            _spriteRenderer.flipX = false;
            if (polvoPasos != null) 
            {
                polvoPasos.transform.localScale = new Vector3(1, 1, 1); 
                polvoPasos.transform.localPosition = new Vector3(-0.7f, -0.98f, 0f); 
            }
            if (puntoAtaque != null && !atacandoVertical) puntoAtaque.localPosition = new Vector3(Mathf.Abs(_puntoAtaqueOffsetBaseX), _puntoAtaqueOffsetBaseY, puntoAtaque.localPosition.z);
            if (wallCheck != null) wallCheck.localPosition = new Vector3(Mathf.Abs(_wallCheckOffsetBaseX), wallCheck.localPosition.y, wallCheck.localPosition.z);
            if (ledgeCheck != null) ledgeCheck.localPosition = new Vector3(Mathf.Abs(_ledgeCheckOffsetBaseX), ledgeCheck.localPosition.y, ledgeCheck.localPosition.z);
        }
        else if (_moveInput.x < 0) 
        {
            _spriteRenderer.flipX = true;
            if (polvoPasos != null) 
            {
                polvoPasos.transform.localScale = new Vector3(-1, 1, 1); 
                polvoPasos.transform.localPosition = new Vector3(0.7f, -0.98f, 0f); 
            }
            if (puntoAtaque != null && !atacandoVertical) puntoAtaque.localPosition = new Vector3(-Mathf.Abs(_puntoAtaqueOffsetBaseX), _puntoAtaqueOffsetBaseY, puntoAtaque.localPosition.z);
            if (wallCheck != null) wallCheck.localPosition = new Vector3(-Mathf.Abs(_wallCheckOffsetBaseX), wallCheck.localPosition.y, wallCheck.localPosition.z);
            if (ledgeCheck != null) ledgeCheck.localPosition = new Vector3(-Mathf.Abs(_ledgeCheckOffsetBaseX), ledgeCheck.localPosition.y, ledgeCheck.localPosition.z);
        }
    }

    private void Atacar()
    {
        if (AudioManager.Instancia != null && sfxAtaque != null) AudioManager.Instancia.ReproducirSFX(sfxAtaque);

        bool esAtaqueHaciaAbajo = !_isGrounded && _moveInput.y < -0.3f;
        bool esAtaqueHaciaArriba = _moveInput.y > 0.3f; 

        if (esAtaqueHaciaAbajo)
        {
            if (_anim != null) { _anim.ResetTrigger("AttackDown"); _anim.SetTrigger("AttackDown"); }
            if (puntoAtaque != null) puntoAtaque.localPosition = new Vector3(0f, -Mathf.Abs(_puntoAtaqueOffsetBaseX), 0f);
        }
        else if (esAtaqueHaciaArriba) 
        {
            if (_anim != null) { _anim.ResetTrigger("AttackUp"); _anim.SetTrigger("AttackUp"); }
            if (puntoAtaque != null) puntoAtaque.localPosition = new Vector3(0f, Mathf.Abs(_puntoAtaqueOffsetBaseX), 0f); 
        }
        else 
        {
            if (_anim != null) { _anim.ResetTrigger("Attack"); _anim.SetTrigger("Attack"); }
            if (puntoAtaque != null)
            {
                float direccionX = _spriteRenderer.flipX ? -1f : 1f;
                puntoAtaque.localPosition = new Vector3(direccionX * Mathf.Abs(_puntoAtaqueOffsetBaseX), _puntoAtaqueOffsetBaseY, 0f);
            }
        }

        if (puntoAtaque != null)
        {
            Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque, capaEnemigos);
            bool heGolpeadoAAlguien = false;

            foreach (Collider2D enemigo in enemigosGolpeados)
            {
                FungnatIA scriptFungnat = enemigo.GetComponent<FungnatIA>();
                if (scriptFungnat != null) { scriptFungnat.RecibirDano(danoAtaque, transform.position); heGolpeadoAAlguien = true; }

                FungusIA scriptFungus = enemigo.GetComponent<FungusIA>();
                if (scriptFungus != null) { scriptFungus.RecibirDano(danoAtaque, transform.position); heGolpeadoAAlguien = true; }

                EnemigoPared scriptPared = enemigo.GetComponent<EnemigoPared>();
                if (scriptPared != null) { scriptPared.RecibirDano(danoAtaque, transform.position); heGolpeadoAAlguien = true; }

                ProyectilPerseguidor proyectil = enemigo.GetComponent<ProyectilPerseguidor>();
                if (proyectil != null) { proyectil.Desviar(); heGolpeadoAAlguien = true; }

                BossFinal scriptBoss = enemigo.GetComponent<BossFinal>();
                if (scriptBoss != null) { scriptBoss.RecibirDano(danoAtaque); heGolpeadoAAlguien = true; }

                RanaRey scriptRana = enemigo.GetComponent<RanaRey>();
                if (scriptRana != null) { scriptRana.RecibirDano(danoAtaque); heGolpeadoAAlguien = true; }
            }

            if (heGolpeadoAAlguien && AudioManager.Instancia != null && sfxImpactoEnemigo != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxImpactoEnemigo);
            }

            if (esAtaqueHaciaAbajo && heGolpeadoAAlguien)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
                _rb.AddForce(Vector2.up * fuerzaRebotePogo, ForceMode2D.Impulse);
            }
        }
    }

    private void IntentarEnganchar()
    {
        Collider2D[] pivotesCercanos = Physics2D.OverlapCircleAll(transform.position, radioGancho, grappleLayer);
        if (pivotesCercanos.Length > 0 && _dj != null)
        {
            Transform pivoteObjetivo = pivotesCercanos[0].transform;
            float distanciaMinima = Vector2.Distance(transform.position, pivoteObjetivo.position);
            foreach(Collider2D col in pivotesCercanos)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if(dist < distanciaMinima) { distanciaMinima = dist; pivoteObjetivo = col.transform; }
            }
            _dj.connectedAnchor = pivoteObjetivo.position;
            _dj.distance = Vector2.Distance(transform.position, pivoteObjetivo.position); 
            _dj.enabled = true;
            if (_lr != null) _lr.enabled = true; 
            _climbState = ClimbState.Swinging;

            if (altavozPasos != null)
            {
                if (sfxGancho != null) altavozPasos.PlayOneShot(sfxGancho);

                if (sfxBalanceoBucle != null)
                {
                    altavozPasos.clip = sfxBalanceoBucle;
                    altavozPasos.loop = true;
                    altavozPasos.Play();
                }
            }
        }
    }
    private void SoltarGancho()
    {
        if (_dj != null) _dj.enabled = false;
        if (_lr != null) _lr.enabled = false; 
        _climbState = ClimbState.None;

        if (altavozPasos != null)
        {
            altavozPasos.Stop();      
            altavozPasos.loop = false; 
        }
    }
    private void SwingingPhysics()
    {
        if (Mathf.Abs(_moveInput.x) > 0.1f) _rb.AddForce(Vector2.right * _moveInput.x * fuerzaBalanceo, ForceMode2D.Force);
    }

    private IEnumerator RutinaDash(Vector2 direccionDash)
    {
    _canDash = false;
    _isDashing = true;

    if (AudioManager.Instancia != null && sfxDash != null) 
    {
        AudioManager.Instancia.ReproducirSFX(sfxDash); 
    }

    StartCoroutine(RutinaEstelaDash()); 
    float gravedadOriginal = _rb.gravityScale; 
    _rb.gravityScale = 0f; 
    _rb.linearVelocity = direccionDash * dashSpeed; 
    
    yield return new WaitForSeconds(dashDuration); 
    
    _rb.gravityScale = gravedadOriginal; 
    _rb.linearVelocity = _rb.linearVelocity * 0.5f; 
    _isDashing = false; 
    
    yield return new WaitForSeconds(dashCooldown); 
    _canDash = true; 
    }
    private IEnumerator RutinaEstelaDash()
    {
        while (_isDashing) { CrearFantasma(); yield return new WaitForSeconds(tiempoEntreFantasmas); }
    }

    private void CrearFantasma()
    {
        GameObject fantasma = new GameObject("DashGhost");
        fantasma.transform.position = transform.position;
        fantasma.transform.localScale = transform.localScale;

        SpriteRenderer sr = fantasma.AddComponent<SpriteRenderer>();
        sr.sprite = _spriteRenderer.sprite; 
        sr.flipX = _spriteRenderer.flipX;
        sr.color = colorEstela;
        sr.sortingLayerID = _spriteRenderer.sortingLayerID;
        sr.sortingOrder = _spriteRenderer.sortingOrder - 1; 

        StartCoroutine(DesvanecerFantasma(sr, fantasma));
    }

    private IEnumerator DesvanecerFantasma(SpriteRenderer sr, GameObject fantasma)
    {
        float tiempo = 0;
        Color colorInicial = sr.color;
        while (tiempo < duracionFantasma)
        {
            tiempo += Time.deltaTime;
            float alfa = Mathf.Lerp(colorInicial.a, 0f, tiempo / duracionFantasma);
            sr.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alfa);
            yield return null;
        }
        Destroy(fantasma);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck) { Gizmos.color = Color.green; Gizmos.DrawWireCube(groundCheck.position, groundCheckSize); }
        if (wallCheck)   { Gizmos.color = Color.red;   Gizmos.DrawWireCube(wallCheck.position,   wallCheckSize);   }
        if (ledgeCheck)  { Gizmos.color = Color.cyan;  Gizmos.DrawWireCube(ledgeCheck.position,  wallCheckSize);   }
        if (puntoAtaque != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque); }
    }

    private void RestaurarVidasUI()
    {
        if (contenedorVida == null || prefabGotaVida == null) return;
        foreach (Transform hijo in contenedorVida.transform) Destroy(hijo.gameObject);
        for (int i = 0; i < vidaMax; i++) Instantiate(prefabGotaVida, contenedorVida.transform);
    }

    private void ReproducirPaso()
    {
        Debug.Log("¡El código ha dado la orden de sonar un paso!");
        if (AudioManager.Instancia == null) return;
        AudioClip[] arrayAUsar = sfxPasosPorDefecto; 

        if (!string.IsNullOrEmpty(_nombreTileActual))
        {
            if (_nombreTileActual.Contains("madera"))
            {
                arrayAUsar = sfxPasosMadera;
            }
            else if (_nombreTileActual.Contains("tierra") || _nombreTileActual == "tileset_1")
            {
                arrayAUsar = sfxPasosHierba; 
            }
        }

        if (arrayAUsar != null && arrayAUsar.Length > 0)
        {
        int indiceAleatorio = Random.Range(0, arrayAUsar.Length);
        
        altavozPasos.clip = arrayAUsar[indiceAleatorio];
        altavozPasos.Play();
        }
    }

    public void SumarGota(int cantidad)
    {
        totalGotas += cantidad;
        if (textoGotasUI != null) textoGotasUI.text = totalGotas.ToString();
    }

    public void RecuperarVida(int cantidad)
    {
        if (vidaActual < vidaMax)
        {
            if (AudioManager.Instancia != null && sfxRecuperarVida != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxRecuperarVida);
            }

            for (int i = 0; i < cantidad; i++)
            {
                if (vidaActual < vidaMax)
                {
                    vidaActual++; 
                    
                    if (contenedorVida != null && prefabGotaVida != null)
                    {
                        Instantiate(prefabGotaVida, contenedorVida.transform);
                    }
                }
            }
        }
    }

    // 👇 ¡AQUÍ ESTÁ LA NUEVA FUNCIÓN AÑADIDA! 👇
    // Al llamarse "Curar", la bolita la detectará sin problemas,
    // y como llama a "RecuperarVida", aprovechará todo tu sistema de UI y sonidos que ya tenías hecho.
    public void Curar(int cantidad)
    {
        RecuperarVida(cantidad);
    }
}