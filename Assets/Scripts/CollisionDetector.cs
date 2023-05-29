using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.NPC;

namespace Scripts
{
    public class CollisionDetector : MonoBehaviour
    {
        public Action OnCollisionDetected;
        public Action<NPCBase> OnTargetHit;

        private bool _detectorEnabled = false;

        public void ActivateDetector()
        {
            _detectorEnabled = true;
        }

        public void DeactivateDetector()
        {
            _detectorEnabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_detectorEnabled)
            {
                return;
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                NPCBase target = other.gameObject.GetComponent<NPCBase>();
                if (target == null || !target.IsAlive())
                {
                    return;
                }
                OnCollisionDetected?.Invoke();
                OnTargetHit?.Invoke(target);
                DeactivateDetector();
            }
        }
    }
}
