using UnityEngine;

public class TrampaPinchos : MonoBehaviour
{
    [Header("Ajustes de Daño")]
    public int danoPinchos = 1;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // ESTO ES LO QUE HACE LA MAGIA DEL TELETRANSPORTE
                player.MuertePorTrampa(danoPinchos); 
            }
        }
    }
}