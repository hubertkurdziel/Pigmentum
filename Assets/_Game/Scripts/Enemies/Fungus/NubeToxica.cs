using UnityEngine;

public class NubeToxica : MonoBehaviour
{
    [Header("Ajustes del Gas")]
    public int danoGas = 1;
    public float tiempoDeVida = 2.5f; 

    private void Start()
    {
        // Establece la autodestrucción automática del objeto tras completar su tiempo de permanencia[cite: 14].
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null) player.TomarDano(danoGas, transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Aplica daño de forma continuada mientras el usuario se encuentre dentro del volumen de colisión[cite: 14].
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null) player.TomarDano(danoGas, transform.position);
        }
    }
}