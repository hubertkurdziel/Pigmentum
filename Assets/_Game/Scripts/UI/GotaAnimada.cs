using UnityEngine;

public class GotaAnimada : MonoBehaviour
{
    private void Start()
    {
        // Inicia la animación en un punto temporal aleatorio para evitar que todos los elementos se muevan de forma sincronizada[cite: 21].
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float tiempoAleatorio = Random.Range(0f, 1f);
            anim.Play("Goteo", 0, tiempoAleatorio);
        }
    }
}