using UnityEngine;

public class MonedaDrop : MonoBehaviour
{
    [Header("Ajustes de la Moneda Pequeña")]
    [SerializeField, Tooltip("Valor que suma al contador de gotas")] 
    private int valorMoneda = 1;
    [SerializeField, Tooltip("Fuerza base con la que salta al salir del enemigo")] 
    private float fuerzaSalto = 6f;

    [Header("Efectos")]
    [SerializeField, Tooltip("Sonido al recoger la moneda")]
    private AudioClip sfxRecogerMoneda; // <-- Aquí guardamos tu sonido

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direccionX = Random.Range(-1f, 1f);
            float direccionY = Random.Range(0.8f, 1.5f);
            Vector2 direccionSalto = new Vector2(direccionX, direccionY).normalized;
            float fuerzaFinal = fuerzaSalto * Random.Range(0.8f, 1.2f);
            
            rb.AddForce(direccionSalto * fuerzaFinal, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController scriptJugador = collision.gameObject.GetComponent<PlayerController>();
            
            if (scriptJugador != null)
            {
                scriptJugador.SumarGota(valorMoneda);
                
                // Le pasamos el sonido al AudioManager justo antes de desaparecer
                if (AudioManager.Instancia != null && sfxRecogerMoneda != null)
                {
                    AudioManager.Instancia.ReproducirSFX(sfxRecogerMoneda);
                }
                
                Destroy(gameObject);
            }
        }
    }
}