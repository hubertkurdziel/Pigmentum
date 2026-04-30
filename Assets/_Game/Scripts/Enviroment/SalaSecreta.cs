using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class SalaSecreta : MonoBehaviour
{
    private Tilemap _tilemap;
    public float velocidadDesvanecimiento = 4f; // Cuanto mayor sea, más rápido desaparece

    void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(CambiarOpacidad(0f)); // 0 = Transparente
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(CambiarOpacidad(1f)); // 1 = Negro opaco
        }
    }

    private IEnumerator CambiarOpacidad(float opacidadObjetivo)
    {
        Color colorActual = _tilemap.color;
        while (Mathf.Abs(colorActual.a - opacidadObjetivo) > 0.05f)
        {
            colorActual.a = Mathf.Lerp(colorActual.a, opacidadObjetivo, Time.deltaTime * velocidadDesvanecimiento);
            _tilemap.color = colorActual;
            yield return null;
        }
        colorActual.a = opacidadObjetivo;
        _tilemap.color = colorActual;
    }
}