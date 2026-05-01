using UnityEngine;

public class VibracionCamara : MonoBehaviour
{
    public static VibracionCamara Instancia;

    private Vector3 _posicionOriginal;
    private float _tiempoRestante;
    private float _fuerzaVibracion;

    void Awake() => Instancia = this;

    // Aplica desplazamientos aleatorios en la jerarquía local para simular un efecto de sacudida[cite: 39].
    void Update()
    {
        if (_tiempoRestante > 0)
        {
            transform.localPosition = _posicionOriginal + (Vector3)Random.insideUnitCircle * _fuerzaVibracion;
            _tiempoRestante -= Time.deltaTime;

            if (_tiempoRestante <= 0) transform.localPosition = _posicionOriginal;
        }
    }

    // Inicializa los parámetros de duración e intensidad para el efecto de movimiento[cite: 39].
    public void EmpezarTerremoto(float duracion, float fuerza)
    {
        _posicionOriginal = transform.localPosition;
        _tiempoRestante = duracion;
        _fuerzaVibracion = fuerza;
    }
}