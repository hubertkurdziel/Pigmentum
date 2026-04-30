using UnityEngine;

public class SalaOculta : MonoBehaviour
{
    [Header("Sonido de Secreto")]
    [Tooltip("El sonido que sonará al descubrir esta sala")]
    [SerializeField] private AudioClip sfxDescubrimiento;

    // Esta variable funciona como un candado para que solo suene la primera vez
    private bool _yaDescubierta = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si entra el jugador y la sala NO ha sido descubierta aún...
        if (collision.CompareTag("Player") && !_yaDescubierta)
        {
            // 1. Ponemos el candado: marcamos la sala como descubierta
            _yaDescubierta = true;

            // 2. Reproducimos el sonido a través de tu fiel AudioManager
            if (AudioManager.Instancia != null && sfxDescubrimiento != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxDescubrimiento);
            }

            // NOTA: Si más adelante quieres que al descubrir la sala 
            // desaparezca una pared negra falsa, el código iría justo aquí.
        }
    }
}