using System.Collections;
using UnityEngine;

public class PelotaSaltarina : MonoBehaviour
{
    [Header("Ajustes de Rebote")]
    [SerializeField] private float fuerzaRebote = 30f;

    [Header("Efecto Visual (Boing)")]
    [SerializeField] private float duracionBoing = 0.2f;
    
    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sfxRebote;

    private Vector3 _escalaOriginal;
    private Coroutine _rutinaBoing;

    void Start()
    {
        _escalaOriginal = transform.localScale;
    }

    // Gestiona la aplicación de fuerza física al objeto en colisión y activa la respuesta sonora[cite: 23].
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (rbJugador != null)
            {
                if (AudioManager.Instancia != null && sfxRebote != null)
                {
                    AudioManager.Instancia.ReproducirSFX(sfxRebote);
                }

                Vector2 direccionRebote = (collision.transform.position - transform.position).normalized;

                rbJugador.linearVelocity = Vector2.zero;
                rbJugador.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);
                
                if (_rutinaBoing != null) StopCoroutine(_rutinaBoing);
                _rutinaBoing = StartCoroutine(RutinaAplastarse());
            }
        }
    }

    // Realiza una deformación elástica mediante la interpolación de la escala para simular un rebote[cite: 23].
    private IEnumerator RutinaAplastarse()
    {
        float tiempo = 0f;
        Vector3 escalaAplastada = new Vector3(_escalaOriginal.x * 1.3f, _escalaOriginal.y * 0.5f, _escalaOriginal.z);
        float tiempoFase = duracionBoing / 2f;

        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / tiempoFase;
            transform.localScale = Vector3.Lerp(_escalaOriginal, escalaAplastada, porcentaje);
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < tiempoFase)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / tiempoFase;
            transform.localScale = Vector3.Lerp(escalaAplastada, _escalaOriginal, porcentaje);
            yield return null;
        }

        transform.localScale = _escalaOriginal;
    }
}