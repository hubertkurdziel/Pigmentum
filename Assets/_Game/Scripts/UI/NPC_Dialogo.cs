using UnityEngine;

public class NPC_Dialogo : MonoBehaviour
{
    [System.Serializable]
    public struct OpcionDialogo
    {
        public string textoDelBoton; 
        [TextArea(2, 4)] public string[] frasesRespuesta; 
        public bool esOpcionDeSalida; 
        
        public bool ocultarTrasLeer; 
    }

    [Header("Presentación Inicial")]
    public string nombre = "Desconocido";
    [TextArea(2, 4)] public string[] saludoInicial;
    
    [Header("Menú de Opciones")]
    public OpcionDialogo[] opciones;

    [Header("Audio del NPC")]
    [Tooltip("El sonido (como un balbuceo corto o tecleo) que sonará mientras habla")]
    public AudioClip sonidoHablar; // <-- NUEVO: Para ponerle voz al NPC
    
    [HideInInspector] public bool[] opcionesLeidas; 

    private bool yaHablo = false;
    private Animator _animator; 
    private SpriteRenderer _spriteRenderer; 
    
    // <-- NUEVO: El componente que reproducirá el sonido
    private AudioSource _audioSource; 

    private void Awake()
    {
        if (opciones != null)
        {
            opcionesLeidas = new bool[opciones.Length];
        }

        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // --- NUEVO: Creamos el "altavoz" y lo configuramos automáticamente ---
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = sonidoHablar;
        _audioSource.loop = true; // Fundamental para que suene de continuo mientras salen letras
        _audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (_spriteRenderer != null && !_spriteRenderer.isVisible)
        {
            _spriteRenderer.flipX = false; 
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !yaHablo)
        {
            if (DialogueManager.Instancia == null)
            {
                Debug.LogError("ERROR: DialogueManager no encontrado en la escena.");
                return;
            }

            if (!DialogueManager.Instancia.DialogoActivo())
            {
                DialogueManager.Instancia.IniciarDialogoConOpciones(this);
            }
        }
    }

    public void MarcarOpcionLeida(int indice)
    {
        if (indice >= 0 && indice < opcionesLeidas.Length)
        {
            opcionesLeidas[indice] = true;
        }
    }

    public void DespedirYDesactivar()
    {
        yaHablo = true; 
        
        BoxCollider2D cuerpoFisico = GetComponent<BoxCollider2D>();
        if (cuerpoFisico != null && !cuerpoFisico.isTrigger)
        {
            cuerpoFisico.enabled = false; 
        }
    }

    public void SetHablando(bool hablando)
    {
        // 1. Manejamos la animación (como ya tenías)
        if (_animator != null)
        {
            _animator.SetBool("isTalking", hablando);
        }
        else
        {
            Debug.LogWarning("⚠️ El script del NPC no encuentra ningún Animator.");
        }

        // --- NUEVO: 2. Manejamos el sonido ---
        if (_audioSource != null && sonidoHablar != null)
        {
            if (hablando)
            {
                // Si el Manager dice que hable, le damos al Play (solo si no estaba sonando ya)
                if (!_audioSource.isPlaying) _audioSource.Play();
            }
            else
            {
                // Si el Manager dice que pare, cortamos el sonido en seco
                if (_audioSource.isPlaying) _audioSource.Stop();
            }
        }
    }
}