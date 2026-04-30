using UnityEngine;

public class VibracionCamara : MonoBehaviour
{
    public static VibracionCamara Instancia;

    private Vector3 _posicionOriginal;
    private float _tiempoRestante;
    private float _fuerzaVibracion;

    void Awake() => Instancia = this;

    void Update()
    {
        if (_tiempoRestante > 0)
        {
            // Movemos la cámara a una posición aleatoria muy rápido
            transform.localPosition = _posicionOriginal + (Vector3)Random.insideUnitCircle * _fuerzaVibracion;
            _tiempoRestante -= Time.deltaTime;

            // Al terminar, la devolvemos a su sitio
            if (_tiempoRestante <= 0) transform.localPosition = _posicionOriginal;
        }
    }

    public void EmpezarTerremoto(float duracion, float fuerza)
    {
        _posicionOriginal = transform.localPosition;
        _tiempoRestante = duracion;
        _fuerzaVibracion = fuerza;
    }
}