using UnityEngine;
using System.Collections;

public class PortalSalida : MonoBehaviour
{
    [Header("Ajustes de Desaparición")]
    [SerializeField, Tooltip("Segundos que el portal se queda abierto y normal")] 
    private float tiempoVisible = 1.5f;
    
    [SerializeField, Tooltip("Segundos que tarda en encogerse hasta desaparecer")] 
    private float tiempoEncogimiento = 0.3f;

    [Header("Efectos de Sonido")]
    [SerializeField, Tooltip("Sonido al aparecer saliendo del portal")]
    private AudioClip sfxSalir;

    private void Start()
    {
        // Reproducimos el sonido de aparición nada más empezar la escena
        if (AudioManager.Instancia != null && sfxSalir != null)
        {
            AudioManager.Instancia.ReproducirSFX(sfxSalir);
        }

        // En cuanto cargue la escena, empieza la cuenta atrás
        StartCoroutine(RutinaCerrarPortal());
    }

    private IEnumerator RutinaCerrarPortal()
    {
        // 1. Esperamos el tiempo que tiene que estar visible
        yield return new WaitForSeconds(tiempoVisible);

        // 2. Lo encogemos poco a poco para un efecto visual bonito
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0f;

        while (tiempo < tiempoEncogimiento)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / tiempoEncogimiento;
            
            // Interpolamos de su tamaño normal a 0
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, porcentaje);
            yield return null;
        }

        // 3. Una vez encogido del todo, borramos el objeto para que no consuma memoria
        Destroy(gameObject);
    }
}