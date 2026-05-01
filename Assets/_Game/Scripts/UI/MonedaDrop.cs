using UnityEngine;

public class MonedaDrop : MonoBehaviour
{
    [Header("Ajustes de la Moneda Pequeña")]
    [SerializeField] private int valorMoneda = 1;
    [SerializeField] private float fuerzaSalto = 6f;

    [Header("Efectos")]
    [SerializeField] private AudioClip sfxRecogerMoneda;

    private void Start()
    {
        // Aplica un impulso físico inicial con dirección y fuerza aleatorias para simular el efecto de expulsión[cite: 20].
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
        // Registra la colisión con el personaje, suma el valor correspondiente al estado y activa la respuesta sonora[cite: 20].
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController scriptJugador = collision.gameObject.GetComponent<PlayerController>();
            
            if (scriptJugador != null)
            {
                scriptJugador.SumarGota(valorMoneda);
                
                if (AudioManager.Instancia != null && sfxRecogerMoneda != null)
                {
                    AudioManager.Instancia.ReproducirSFX(sfxRecogerMoneda);
                }
                
                Destroy(gameObject);
            }
        }
    }
}