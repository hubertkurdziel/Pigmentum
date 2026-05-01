using UnityEngine;

public class ColeccionableGota : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int valor = 1; 
    [SerializeField] private GameObject efectoExplosionPrefab;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sfxRecoger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Registra la obtención del elemento en el estado del usuario y activa los efectos de retroalimentación antes de su eliminación[cite: 19].
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SumarGota(valor);
            }

            if (efectoExplosionPrefab != null)
            {
                Instantiate(efectoExplosionPrefab, transform.position, Quaternion.identity);
            }

            if (AudioManager.Instancia != null && sfxRecoger != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxRecoger);
            }

            Destroy(gameObject);
        }
    }
}