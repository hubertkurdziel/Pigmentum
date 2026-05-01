using UnityEngine;

public class TrampaPinchos : MonoBehaviour
{
    [Header("Ajustes de Daño")]
    public int danoPinchos = 1;

    // Detecta la colisión persistente con el usuario para activar la lógica de daño y reubicación[cite: 24].
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.MuertePorTrampa(danoPinchos); 
            }
        }
    }
}