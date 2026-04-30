using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFinDemo : MonoBehaviour
{
    [Header("Configuración Visual")]
    public CanvasGroup grupoCanvas;
    // He quitado la variable "tiempoDeFundido" porque ya no nos hace falta

    [Header("Música")]
    [Tooltip("La canción de victoria o créditos al morir el boss")]
    public AudioClip musicaFinal; 

    private void Start()
    {
        // Nos aseguramos de que empiece invisible
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
        // 1. Esperamos los 15 segundos para que el jugador recoja todo
        yield return new WaitForSeconds(espera);

        // --- ¡EL CORTE DIRECTO! ---

        // 2. Congelamos todo el juego al instante
        Time.timeScale = 0f;

        // 3. Pantallazo en negro con el texto "FIN DEMO" al instante (de 0 a 1 de golpe)
        if (grupoCanvas != null) 
        {
            grupoCanvas.alpha = 1f;
            grupoCanvas.blocksRaycasts = true;
        }

        // 4. Cambiamos la música en ese mismo frame usando tu AudioManager
        if (AudioManager.Instancia != null && musicaFinal != null)
        {
            AudioManager.Instancia.ReproducirMusica(musicaFinal);
        }

        // 5. Dejamos que el jugador lea el texto y escuche la música durante unos segundos
        // (Usa WaitForSecondsRealtime porque el juego está pausado)
        yield return new WaitForSecondsRealtime(5f);

        // 6. Restauramos el tiempo y le devolvemos al menú
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}