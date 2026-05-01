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
        
        posicionInicialFondo = transform.position;
        posicionInicialCamara = camaraTransform.position;
    }

    // Calcula y aplica la posición del objeto basándose en el desplazamiento relativo de la cámara[cite: 22].
    void FixedUpdate()
    {
        Vector3 distanciaRecorrida = (camaraTransform.position - posicionInicialCamara) * multiplicadorParallax;

        transform.position = new Vector3(
            posicionInicialFondo.x + distanciaRecorrida.x, 
            posicionInicialFondo.y + distanciaRecorrida.y, 
            transform.position.z);
    }
}