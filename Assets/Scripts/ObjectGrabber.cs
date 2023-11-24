using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


namespace Fineallday
{
    public class ObjectGrabber : MonoBehaviour
    {
        private PlayerInputManager inputs;
        
        void Awake()
        {
            inputs = new PlayerInputManager();
            inputs.MouseInteractions.Pickup.performed += Interact;
        }

        private void Interact(InputAction.CallbackContext value)
        {
            //un raycast
            //var point = value.ReadValue<Vector3>();
            
            var m = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(m);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                
                //Si item
                //récupérer le Scriptable de l'item
                //ajouter le scriptable à l'inventaire du player
                
                
               // Debug.Log(hit.collider.name);
            }
           
        }

        private void OnEnable()
        {
            inputs.MouseInteractions.Pickup.Enable();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
