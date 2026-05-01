using UnityEngine;
using TMPro;

public class ContadorFPS : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoFPS;

    private float deltaTime = 0.0f;
    private bool mostrarFPS = false;

    private void Awake()
    {
        // Gestiona la persistencia del objeto entre escenas y elimina duplicados[cite: 30].
        ContadorFPS[] otrosContadores = FindObjectsOfType<ContadorFPS>();

        if (otrosContadores.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ActualizarEstado();
    }

    private void Update()
    {
        // Calcula el promedio de cuadros por segundo basándose en el tiempo transcurrido independiente de la escala[cite: 30].
        if (!mostrarFPS || textoFPS == null) return;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        textoFPS.text = Mathf.Ceil(fps).ToString() + " FPS";
    }

    public void ActualizarEstado()
    {
        // Sincroniza la visibilidad del componente con las preferencias guardadas en el sistema[cite: 30].
        mostrarFPS = PlayerPrefs.GetInt("MostrarFPS", 0) == 1;

        if (textoFPS != null)
        {
            textoFPS.gameObject.SetActive(mostrarFPS);
        }
    }
}