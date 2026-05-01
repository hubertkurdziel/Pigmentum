using UnityEngine;

public class PersistenciaBrillo : MonoBehaviour
{
    private void Awake()
    {
        // Evita la duplicación del objeto al cambiar de escena y garantiza su persistencia[cite: 36].
        if (FindObjectsOfType<PersistenciaBrillo>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
}