using UnityEngine;

// Este script se adjunta al objeto que quieres que sea interactuable (tu bot�n)
// Necesita tener un Collider para ser detectado por el Raycast.
public class ButtonProperties : MonoBehaviour
{
    [Header("Objeto a Modificar")]
    [Tooltip("Arrastra aqu� el objeto cuyo Box Collider quieres cambiar a Trigger.")]
    public BoxCollider objectToModifyCollider; // Referencia al collider del OTRO objeto

    [Header("Objeto a Desactivar")]
    [Tooltip("Arrastra aqu� el objeto que quieres desactivar al presionar el bot�n.")]
    public GameObject objectToDeactivate;  // Nuevo: Referencia al GameObject a desactivar

    // Puedes a�adir m�s propiedades aqu� si el bot�n necesita hacer m�s cosas
    // Por ejemplo: sonido de interacci�n, animaci�n, etc.

    // Este m�todo puede ser llamado por el script Interactor
    // para ejecutar la acci�n principal del bot�n
    public void Interact()
    {
        if (objectToModifyCollider != null)
        {
            // Cambia el isTrigger del collider referenciado a true
            objectToModifyCollider.isTrigger = true;
            Debug.Log("�Bot�n presionado! El collider del objeto referenciado ahora es Trigger.");

            // Desactiva el GameObject si est� asignado
            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
            }

            // Aqu� podr�as a�adir m�s l�gica, como reproducir un sonido o animaci�n
            // GetComponent<AudioSource>()?.Play();
        }
        else
        {
            Debug.LogWarning("ButtonProperties en " + gameObject.name + " no tiene un BoxCollider referenciado para modificar.");
        }
    }
}
