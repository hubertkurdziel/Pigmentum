using UnityEngine;

public class GotaAnimada : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            // Cogemos el estado actual de la animación y le decimos
            // que empiece en un punto aleatorio entre 0 y 1 (0% y 100%)
            float tiempoAleatorio = Random.Range(0f, 1f);
            
            // "Goteo" debe ser el nombre exacto de tu estado en el Animator
            anim.Play("Goteo", 0, tiempoAleatorio);
        }
    }
}