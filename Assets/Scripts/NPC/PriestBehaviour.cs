using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.NPC
{
    public class PriestBehaviour : NPCBase
    {
        [SerializeField] private int reviveCooldown;

        private NPCBase _allie;
        private bool _canRevive = true;

        private Coroutine _reviveCoroutine;

        protected override void Initialize()
        {
            base.Initialize();
            OnFindTarget += AggressiveBehaviour;
        }

        protected override void Awaken()
        {
            base.Awaken();
            UpdateTarget();
            AggressiveToPlayer();
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isAlive && !_isBusy && _canRevive && other.gameObject.CompareTag("Enemy"))
            {
                var allie = wavesHolder.GetNPC(other.gameObject);

                if (!allie.IsAlive())
                {
                    ReviveNPC(allie);
                }
            }
        }

        private void StopReviving()
        {
            if (_reviveCoroutine != null)
            {
                StopCoroutine(_reviveCoroutine);
                _reviveCoroutine = null;
            }
        }

        private void ReviveNPC(NPCBase allie)
        {
            StopReviving();
            _allie = allie;
            _reviveCoroutine = StartCoroutine(ReviveAllie());
        }

        private IEnumerator ReviveAllie()
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                _isBusy = true;
                _canRevive = false;
                WalkToPoint(_allie.transform.position);
                yield return new WaitUntil(() => Vector3.Distance(transform.position, _allie.transform.position) <= agent.stoppingDistance);
                FaceTo(_allie.transform);
                particlesHolder.OnCast();
                animator.Cast();
            }
        }

        public override void Despawn()
        {
            base.Despawn();
            OnFindTarget -= AggressiveBehaviour;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnFindTarget -= AggressiveBehaviour;
        }

        public override void OnCastEnd()
        {
            base.OnCastEnd();
            _allie.Revive();
            StopLookTarget();
            _isBusy = false;
            StartCoroutine(ReviveReduction());
        }

        private IEnumerator ReviveReduction()
        {
            yield return new WaitForSeconds(reviveCooldown);
            _canRevive = true;
        }
    }
}
