using System.Collections;
using UnityEngine;

public class PelotaSaltarina : MonoBehaviour
{
    [Header("Ajustes de Rebote")]
    [SerializeField, Tooltip("Fuerza con la que expulsa al jugador")] 
    private float fuerzaRebote = 30f;

    [Header("Efecto Visual (Boing)")]
    [SerializeField, Tooltip("Duración total de la animación de aplastarse y volver")]
    private float duracionBoing = 0.2f;
    
    [Header("Efectos de Sonido")]
    [SerializeField, Tooltip("Sonido que se reproduce al chocar")]
    private AudioClip sfxRebote;

    private Vector3 _escalaOriginal;
    private Coroutine _rutinaBoing;

    private void Start()
    {
        // Guardamos la escala inicial que tiene la pelota en el editor
        _escalaOriginal = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos si el objeto que nos ha chocado tiene la etiqueta de Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Agarramos las físicas del jugador
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (rbJugador != null)
            {
                // Reproducir el sonido al chocar
                if (AudioManager.Instancia != null && sfxRebote != null)
                {
                    AudioManager.Instancia.ReproducirSFX(sfxRebote);
                }

                // --- NUEVO: CÁLCULO DE DIRECCIÓN ---
                // Calculamos la dirección restando la posición del jugador menos la de la seta.
                // ".normalized" hace que la fuerza siempre sea igual de fuerte, sin importar la distancia.
                Vector2 direccionRebote = (collision.transform.position - transform.position).normalized;

                // 1. Frenamos al jugador en seco (tanto en X como en Y) para que el rebote sea limpio y potente.
                rbJugador.linearVelocity = Vector2.zero;
                
                // 2. Le pegamos el impulso en la dirección que hemos calculado
                rbJugador.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);
                
                // --- Iniciamos el efecto visual ---
                if (_rutinaBoing != null) StopCoroutine(_rutinaBoing);
                _rutinaBoing = StartCoroutine(RutinaAplastarse());
            }
        }
    }

    private IEnumerator RutinaAplastarse()
    {
        float tiempo = 0f;
        
        // Calculamos la escala aplastada: un 30% más ancha y un 50% más bajita
        Vector3 escalaAplastada = new Vector3(_escalaOriginal.x * 1.3f, _escalaOriginal.y * 0.5f, _escalaOriginal.z);
        
        // El tiempo se divide en dos: la mitad para bajar y la mitad para subir
        float tiempoFase = duracionBoing / 2f;

        // Fase 1: Aplastarse contra el suelo
        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / tiempoFase;
            // Interpolamos desde la forma original a la forma de tortilla
            transform.localScale = Vector3.Lerp(_escalaOriginal, escalaAplastada, porcentaje);
            yield return null;
        }

        // Fase 2: Volver a su forma de siempre como un muelle
        tiempo = 0f;
        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / tiempoFase;
            // Interpolamos desde la forma de tortilla a la forma original
            transform.localScale = Vector3.Lerp(escalaAplastada, _escalaOriginal, porcentaje);
            yield return null;
        }

        // Medida de seguridad: nos aseguramos de que termine EXACTAMENTE en la escala original
        transform.localScale = _escalaOriginal;
    }
}