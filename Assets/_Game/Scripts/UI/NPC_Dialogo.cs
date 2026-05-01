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
    public AudioClip sonidoHablar; 
    
    [HideInInspector] public bool[] opcionesLeidas; 

    private bool yaHablo = false;
    private Animator _animator; 
    private SpriteRenderer _spriteRenderer; 
    private AudioSource _audioSource; 

    private void Awake()
    {
        // Inicializa el registro de opciones y configura el componente de audio.
        if (opciones != null)
        {
            opcionesLeidas = new bool[opciones.Length];
        }

        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = sonidoHablar;
        _audioSource.loop = true; 
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
        // Activa la interfaz de diálogo a través del manager si se cumplen las condiciones de proximidad.
        if (col.CompareTag("Player") && !yaHablo)
        {
            if (DialogueManager.Instancia != null)
            {
                if (!DialogueManager.Instancia.DialogoActivo())
                {
                    DialogueManager.Instancia.IniciarDialogoConOpciones(this);
                }
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
        // Inhabilita la interacción futura y la colisión física del objeto.
        yaHablo = true; 
        
        BoxCollider2D cuerpoFisico = GetComponent<BoxCollider2D>();
        if (cuerpoFisico != null && !cuerpoFisico.isTrigger)
        {
            cuerpoFisico.enabled = false; 
        }
    }

    public void SetHablando(bool hablando)
    {
        // Gestiona los estados de animación y la reproducción de la pista de audio.
        if (_animator != null)
        {
            _animator.SetBool("isTalking", hablando);
        }

        if (_audioSource != null && sonidoHablar != null)
        {
            if (hablando)
            {
                if (!_audioSource.isPlaying) _audioSource.Play();
            }
            else
            {
                if (_audioSource.isPlaying) _audioSource.Stop();
            }
        }
    }
}