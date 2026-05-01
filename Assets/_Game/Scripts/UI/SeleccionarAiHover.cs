using UnityEngine;
using UnityEngine.EventSystems;

// Gestiona el foco del sistema de navegación mediante la interacción del cursor con los elementos de la interfaz[cite: 38].
public class SeleccionarAlHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}