using UnityEngine;

public class PersistenciaBrillo : MonoBehaviour
{
    private void Awake()
    {
        // Si venimos de otra escena y ya hay un Canvas de brillo, destruimos este para no tener dos capas negras sumándose
        if (FindObjectsOfType<PersistenciaBrillo>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        // La magia para que el brillo viaje contigo al nivel
        DontDestroyOnLoad(gameObject);
    }
}