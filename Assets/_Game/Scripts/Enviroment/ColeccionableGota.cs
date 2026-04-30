using UnityEngine;

public class ColeccionableGota : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int valor = 1; 
    [SerializeField] private GameObject efectoExplosionPrefab; // Arrastra aquí tu prefab de partículas

    [Header("Efectos de Sonido")]
    [SerializeField, Tooltip("Sonido al recoger este coleccionable")]
    private AudioClip sfxRecoger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si lo que nos toca tiene el Tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Buscamos el script del Player para sumarle la gota
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SumarGota(valor);
            }

            // Instanciamos la explosión en la misma posición que la moneda
            if (efectoExplosionPrefab != null)
            {
                Instantiate(efectoExplosionPrefab, transform.position, Quaternion.identity);
            }

            // Reproducimos el sonido a través del AudioManager justo antes de desaparecer
            if (AudioManager.Instancia != null && sfxRecoger != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxRecoger);
            }

            // Destruimos la moneda para que desaparezca
            Destroy(gameObject);
        }
    }
}