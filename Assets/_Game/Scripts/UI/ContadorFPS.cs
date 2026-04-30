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
        // 1. Buscamos si ya existe otro contador igual
        ContadorFPS[] otrosContadores = FindObjectsOfType<ContadorFPS>();

        if (otrosContadores.Length > 1)
        {
            // Si venimos de otra escena y ya había uno, destruimos la copia nueva
            Destroy(gameObject);
            return;
        }

        // 2. Le decimos a Unity que NO destruya el Canvas de los FPS al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ActualizarEstado();
    }

    private void Update()
    {
        if (!mostrarFPS || textoFPS == null) return;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        textoFPS.text = Mathf.Ceil(fps).ToString() + " FPS";
    }

    public void ActualizarEstado()
    {
        // Leemos la memoria del juego para saber si el usuario marcó el Toggle
        mostrarFPS = PlayerPrefs.GetInt("MostrarFPS", 0) == 1;

        if (textoFPS != null)
        {
            textoFPS.gameObject.SetActive(mostrarFPS);
        }
    }
}