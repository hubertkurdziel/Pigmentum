using UnityEngine;

public class SalaOculta : MonoBehaviour
{
    [SerializeField] private AudioClip sfxDescubrimiento;
    private bool _yaDescubierta = false;

    // Controla la activación única de un evento sonoro al detectar la entrada del usuario en el área[cite: 37].
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_yaDescubierta)
        {
            _yaDescubierta = true;

            if (AudioManager.Instancia != null && sfxDescubrimiento != null)
            {
                AudioManager.Instancia.ReproducirSFX(sfxDescubrimiento);
            }
        }
    }
}