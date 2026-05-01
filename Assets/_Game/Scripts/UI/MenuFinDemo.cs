using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFinDemo : MonoBehaviour
{
    [Header("Configuración Visual")]
    public CanvasGroup grupoCanvas;

    [Header("Música")]
    public AudioClip musicaFinal; 

    private void Start()
    {
        // Inicializa el componente de interfaz como invisible e inactivo para la interacción[cite: 33].
        if (grupoCanvas != null)
        {
            grupoCanvas.alpha = 0f;
            grupoCanvas.blocksRaycasts = false;
        }
    }

    public void IniciarSecuenciaFin(float espera)
    {
        StartCoroutine(SecuenciaFin(espera));
    }

    private IEnumerator SecuenciaFin(float espera)
    {
        // Ejecuta la transición final pausando el tiempo del sistema y activando la interfaz de cierre[cite: 33].
        yield return new WaitForSeconds(espera);

        Time.timeScale = 0f;

        if (grupoCanvas != null) 
        {
            grupoCanvas.alpha = 1f;
            grupoCanvas.blocksRaycasts = true;
        }

        if (AudioManager.Instancia != null && musicaFinal != null)
        {
            AudioManager.Instancia.ReproducirMusica(musicaFinal);
        }

        // Utiliza tiempo real para la espera debido a la pausa en la escala de tiempo del motor[cite: 33].
        yield return new WaitForSecondsRealtime(5f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}