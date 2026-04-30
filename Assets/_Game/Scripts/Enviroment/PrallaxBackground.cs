using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startPos;
    public GameObject cam; // Aquí arrastra la "Main Camera"
    
    [Tooltip("0 = Se mueve con la cámara (estático), 1 = No se mueve (el jugador lo adelanta rápido)")]
    public float parallaxMultiplier; 

    void Start()
    {
        // Guardamos la posición inicial en X
        startPos = transform.position.x;
        
        // Verificación de seguridad por si te olvidas de arrastrar la cámara
        if (cam == null) cam = Camera.main.gameObject;
    }

    // Usamos FixedUpdate porque tu PlayerController usa físicas
    // Esto elimina los tirones y vibraciones al caminar
    void FixedUpdate() 
    {
        // Calculamos la distancia basada en el multiplicador
        float dist = (cam.transform.position.x * (1 - parallaxMultiplier));

        // Movemos el CONTENEDOR PADRE completo
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
    }
}