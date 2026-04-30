using UnityEngine;

public class ParallaxLayerSecuencial : MonoBehaviour
{
    [Header("Configuración")]
    public Transform camaraTransform;
    public float multiplicadorParallax;

    private Vector3 posicionInicialFondo;
    private Vector3 posicionInicialCamara;

    void Start()
    {
        if (camaraTransform == null)
        {
            camaraTransform = Camera.main.transform;
        }
        
        // Guardamos las posiciones exactas donde empiezan el fondo y la cámara
        posicionInicialFondo = transform.position;
        posicionInicialCamara = camaraTransform.position;
    }

    // Usamos FixedUpdate o LateUpdate dependiendo de cómo se mueva tu personaje
    void FixedUpdate()
    {
        // Calculamos la distancia total absoluta que ha recorrido la cámara
        Vector3 distanciaRecorrida = (camaraTransform.position - posicionInicialCamara) * multiplicadorParallax;

        // Asignamos la posición exacta basándonos en el inicio + la distancia
        transform.position = new Vector3(
            posicionInicialFondo.x + distanciaRecorrida.x, 
            posicionInicialFondo.y + distanciaRecorrida.y, 
            transform.position.z);
    }
}