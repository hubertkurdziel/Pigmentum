using System.Collections;
using UnityEngine;

public class RanaRey : MonoBehaviour
{
    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMaxima = 5;
    private int _vidaActual;

    [Header("Drops (Piñata)")]
    [SerializeField] private GameObject prefabMonedaDorada;
    [SerializeField] private int monedasPorGolpe = 3;
    [SerializeField] private int monedasMuerte = 50;

    [Header("Rebote y Squash (Boing)")]
    [SerializeField] private float fuerzaRebote = 30f;
    [SerializeField] private float duracionSquash = 0.2f;
    [SerializeField] private float factorAchateY = 0.5f;
    [SerializeField] private float factorEnsancheX = 1.3f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sfxInflar;

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

    // Procesa la colisión con el jugador para aplicar un impulso de rebote y reducir la salud.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_estaMuerta) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                Vector2 direccionRebote = (collision.transform.position - transform.position).normalized;
                
                if (direccionRebote.y < 0.5f) direccionRebote.y = 1f; 
                direccionRebote.Normalize();

                rbJugador.linearVelocity = Vector2.zero;
                rbJugador.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);

                RecibirDano(1);
            }
        }
    }

    // Gestiona la reducción de vida, la activación de efectos visuales y la lógica de muerte.
    public void RecibirDano(int cantidad)
    {
        if (_estaMuerta) return;

        _vidaActual -= cantidad;

        if (_rutinaSquash != null) StopCoroutine(_rutinaSquash);
        _rutinaSquash = StartCoroutine(RutinaAplastarse());

        if (_vidaActual > 0)
        {
            SoltarMonedas(monedasPorGolpe);
        }
        else
        {
            Morir();
        }
    }

    // Instancia objetos de recompensa y les aplica fuerzas físicas aleatorias para su dispersión.
    private void SoltarMonedas(int cantidad)
    {
        if (prefabMonedaDorada == null) return;

        for (int i = 0; i < cantidad; i++)
        {
            GameObject moneda = Instantiate(prefabMonedaDorada, transform.position + Vector3.up * 1f, Quaternion.identity);
            Rigidbody2D rbMoneda = moneda.GetComponent<Rigidbody2D>();
            
            if (rbMoneda != null)
            {
                float dirX = Random.Range(-1f, 1f);
                float dirY = Random.Range(0.8f, 1.5f);
                float fuerza = Random.Range(10f, 25f);

                rbMoneda.AddForce(new Vector2(dirX, dirY).normalized * fuerza, ForceMode2D.Impulse);
            }
        }
    }

    // Finaliza el estado del objeto, desactiva componentes visuales y genera el botín final.
    private void Morir()
    {
        _estaMuerta = true;

        if (coronaObj != null) coronaObj.SetActive(false);

        SoltarMonedas(monedasMuerte);
        Destroy(gameObject, 0.1f);
    }

    // Realiza una deformación elástica mediante la interpolación de la escala y activa sonidos asociados.
    private IEnumerator RutinaAplastarse()
    {
        float tiempo = 0f;
        Vector3 escalaAplastada = new Vector3(_escalaOriginal.x * factorEnsancheX, _escalaOriginal.y * factorAchateY, _escalaOriginal.z);
        float tiempoFase = duracionSquash / 2f;

        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_escalaOriginal, escalaAplastada, tiempo / tiempoFase);
            yield return null;
        }

        if (AudioManager.Instancia != null && sfxInflar != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxInflar);
        }

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