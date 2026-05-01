using UnityEngine;
using System.Collections;

public class PortalSalida : MonoBehaviour
{
    [SerializeField] private float tiempoVisible = 1.5f;
    [SerializeField] private float tiempoEncogimiento = 0.3f;
    [SerializeField] private AudioClip sfxSalir;

    private void Start()
    {
        if (AudioManager.Instancia != null && sfxSalir != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxSalir);
        }

        StartCoroutine(RutinaCerrarPortal());
    }

    // Controla el tiempo de permanencia y la interpolación de escala hasta la eliminación del objeto en escena.
    private IEnumerator RutinaCerrarPortal()
    {
        yield return new WaitForSeconds(tiempoVisible);

        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0f;

        while (tiempo < tiempoEncogimiento)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / tiempoEncogimiento;
            
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, porcentaje);
            yield return null;
        }

        Destroy(gameObject);
    }
}