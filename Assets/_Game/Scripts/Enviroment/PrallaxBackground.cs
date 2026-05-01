using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxMultiplier; 

    void Start()
    {
        startPos = transform.position.x;
        
        if (cam == null) cam = Camera.main.gameObject;
    }

    // Calcula el desfase de posición en el eje X en relación al movimiento de la cámara para simular profundidad[cite: 25].
    void FixedUpdate() 
    {
        float dist = (cam.transform.position.x * (1 - parallaxMultiplier));
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
    }
}