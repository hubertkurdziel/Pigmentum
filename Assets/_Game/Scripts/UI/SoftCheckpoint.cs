using UnityEngine;

public class SoftCheckpoint : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform puntoDeAparicion; 

    // Actualiza la posición de reaparición temporal en el sistema central al detectar la colisión con el jugador.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puntoDeAparicion != null)
        {
            if (GameManager.Instancia != null)
            {
                GameManager.Instancia.GuardarSoftCheckpoint(puntoDeAparicion.position);
            }
        }
    }
}