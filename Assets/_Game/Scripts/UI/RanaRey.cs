using System.Collections;
using UnityEngine;

public class RanaRey : MonoBehaviour
{
    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMaxima = 5;
    private int _vidaActual;

    [Header("Drops (Piñata)")]
    [SerializeField] private GameObject prefabMonedaDorada;
    [SerializeField, Tooltip("Monedas que suelta cada vez que le pegas")] private int monedasPorGolpe = 3;
    [SerializeField, Tooltip("Monedas que escupe al explotar")] private int monedasMuerte = 50;

    [Header("Rebote y Squash (Boing)")]
    [SerializeField] private float fuerzaRebote = 30f;
    [SerializeField] private float duracionSquash = 0.2f;
    [SerializeField] private float factorAchateY = 0.5f;
    [SerializeField] private float factorEnsancheX = 1.3f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sfxInflar; // <-- NUEVO: Sonido para cuando recupera su forma

    // --- NUEVO: Hueco para la corona ---
    [Header("Cosméticos")]
    [SerializeField] private GameObject coronaObj;

    private Vector3 _escalaOriginal;
    private Coroutine _rutinaSquash;
    private bool _estaMuerta = false;

    private void Start()
    {
        _vidaActual = vidaMaxima;
        _escalaOriginal = transform.localScale;
    }

    // Si el jugador le salta en la cabeza, rebotará y la rana recibirá 1 de daño
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_estaMuerta) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                Vector2 direccionRebote = (collision.transform.position - transform.position).normalized;
                
                // Forzamos que siempre empuje un poco hacia arriba para que sea cómodo saltar
                if (direccionRebote.y < 0.5f) direccionRebote.y = 1f; 
                direccionRebote.Normalize();

                rbJugador.linearVelocity = Vector2.zero;
                rbJugador.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);

                // Recibe 1 de daño por el pisotón
                RecibirDano(1);
            }
        }
    }

    // Esta función la llamará tu espada
    public void RecibirDano(int cantidad)
    {
        if (_estaMuerta) return;

        _vidaActual -= cantidad;

        // 1. Efecto visual de boing (squash & stretch)
        if (_rutinaSquash != null) StopCoroutine(_rutinaSquash);
        _rutinaSquash = StartCoroutine(RutinaAplastarse());

        if (_vidaActual > 0)
        {
            // 2. Suelta el "cambio suelto" por el golpe
            SoltarMonedas(monedasPorGolpe);
        }
        else
        {
            // 3. Muere y suelta el premio gordo
            Morir();
        }
    }

    private void SoltarMonedas(int cantidad)
    {
        if (prefabMonedaDorada == null) return;

        for (int i = 0; i < cantidad; i++)
        {
            GameObject moneda = Instantiate(prefabMonedaDorada, transform.position + Vector3.up * 1f, Quaternion.identity);
            Rigidbody2D rbMoneda = moneda.GetComponent<Rigidbody2D>();
            
            if (rbMoneda != null)
            {
                // Dirección en forma de "fuente" hacia arriba y los lados
                float dirX = Random.Range(-1f, 1f);
                float dirY = Random.Range(0.8f, 1.5f);
                float fuerza = Random.Range(10f, 25f); // Fuerza aleatoria para que se dispersen

                rbMoneda.AddForce(new Vector2(dirX, dirY).normalized * fuerza, ForceMode2D.Impulse);
            }
        }
    }

    private void Morir()
    {
        _estaMuerta = true;

        // --- NUEVO: Apagamos la corona instantáneamente ---
        if (coronaObj != null) coronaObj.SetActive(false);

        SoltarMonedas(monedasMuerte);
        Destroy(gameObject, 0.1f);
    }

    private IEnumerator RutinaAplastarse()
    {
        float tiempo = 0f;
        Vector3 escalaAplastada = new Vector3(_escalaOriginal.x * factorEnsancheX, _escalaOriginal.y * factorAchateY, _escalaOriginal.z);
        float tiempoFase = duracionSquash / 2f;

        // Fase 1: Aplastarse (Squash)
        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_escalaOriginal, escalaAplastada, tiempo / tiempoFase);
            yield return null;
        }

        // --- NUEVO: Reproducir sonido justo cuando empieza a hincharse ---
        if (AudioManager.Instancia != null && sfxInflar != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxInflar);
        }

        // Fase 2: Volver a su tamaño original (Hinchándose)[cite: 12]
        tiempo = 0f;
        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaAplastada, _escalaOriginal, tiempo / tiempoFase);
            yield return null;
        }

        transform.localScale = _escalaOriginal;
    }
}