using UnityEngine;

public class NubeToxica : MonoBehaviour
{
    [Header("Ajustes del Gas")]
    public int danoGas = 1;
    public float tiempoDeVida = 2.5f; 

    private void Start()
    {
        // La nube se autodestruye cuando pasa su tiempo de vida
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
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null) player.TomarDano(danoGas, transform.position);
        }
    }
}