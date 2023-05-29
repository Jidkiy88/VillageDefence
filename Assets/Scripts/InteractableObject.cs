using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class InteractableObject : MonoBehaviour
    {
        public Action OnInteract;
        public void Interact()
        {
            OnInteract?.Invoke();
        }
    }
}
