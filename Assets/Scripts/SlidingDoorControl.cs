using UnityEngine;

public class ControlPuertaCorrediza : MonoBehaviour
{
    private Animator animator;
    private bool puertaAbierta = false;
    private string tagPersonaje = "Player"; // Etiqueta (Tag) que debe tener tu personaje

    void Start()
    {
        // Obt�n la referencia al componente Animator del objeto "SlidingDoor"
        animator = GetComponentInParent<Animator>();

        // Aseg�rate de que el Animator existe
        if (animator == null)
        {
            Debug.LogError("El objeto " + gameObject.name + " no tiene un componente Animator.");
            enabled = false; // Desactiva el script si no hay Animator
        }
    }

    // Este m�todo se llama cuando otro Collider entra en el Trigger de este objeto
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entr� en el Trigger es el personaje
        if (other.CompareTag(tagPersonaje) && !puertaAbierta)
        {
            // Si la puerta est� cerrada y el personaje entra, �brela
            puertaAbierta = true;
            animator.SetBool("Abrir", puertaAbierta);
        }
    }

    // Este m�todo se llama cuando otro Collider sale del Trigger de este objeto
    private void OnTriggerExit(Collider other)
{
    Debug.Log("OnTriggerExit llamado por: " + other.gameObject.name + " con etiqueta: " + other.tag);
    // Verifica si el objeto que sali� del Trigger es el personaje y si la puerta est� abierta
    if (other.CompareTag(tagPersonaje) && puertaAbierta)
    {
        puertaAbierta = false;
        animator.SetBool("Abrir", puertaAbierta);
        Debug.Log("Puerta Abierta (Cerrando): " + puertaAbierta);
    }
}
}
