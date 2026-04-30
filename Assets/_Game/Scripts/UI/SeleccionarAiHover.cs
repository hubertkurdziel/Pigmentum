using UnityEngine;
using UnityEngine.EventSystems;

// Añadimos IPointerExitHandler para detectar cuando el ratón se va del botón
public class SeleccionarAlHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. Forzamos a vaciar la selección anterior para que no se queden dos marcados
        EventSystem.current.SetSelectedGameObject(null);
        
        // 2. Seleccionamos el actual
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 3. Cuando el ratón sale del botón, lo deseleccionamos visualmente
        EventSystem.current.SetSelectedGameObject(null);
    }
}