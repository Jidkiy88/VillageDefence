using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.NPC
{
    public class PlagueDoctor : NPCBase
    {
        [SerializeField] private float healValue;
        [SerializeField] private float healCooldown;

        private NPCBase _allie;
        private bool _canHeal = true;

        private Coroutine _healCoroutine;

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

        protected override void Death()
        {
            base.Death();
            RewardByDeath();
            StopAggressiveToPlayer();
        }

        private void OnTriggerStay(Collider other)
        {
            if (_canHeal && !_isBusy && _isAlive && other.gameObject.CompareTag("Enemy"))
            {
                _allie = wavesHolder.GetNPC(other.gameObject);

                if (_allie == null || !_allie.IsAlive())
                {
                    return;
                }

                if (_allie.IsWounded())
                {
                    HealNPC();
                }
            }
        }

        private void HealNPC()
        {
            StopHealing();
            _isBusy = true;
            _canHeal = false;
            _healCoroutine = StartCoroutine(HealAllie());
            StopNPC();
        }

        private void StopHealing()
        {
            if (_healCoroutine != null)
            {
                StopCoroutine(_healCoroutine);
                _healCoroutine = null;
                _allie = null;
            }
        }

        private IEnumerator HealAllie()
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                yield break;
            }

            LookTarget(_allie.transform);
            particlesHolder.OnCast();
            animator.Cast();
        }

        public override void OnCastEnd()
        {
            base.OnCastEnd();
            StopLookTarget();
            _allie.GetHeal(healValue);
            StartCoroutine(HealReduction());
        }

        public override void Despawn()
        {
            base.Despawn();
            OnFindTarget -= AggressiveBehaviour;
        }

        private IEnumerator HealReduction()
        {
            yield return new WaitForSeconds(healCooldown);
            _canHeal = true;
        }

        protected override void OnDestroy()
        {
            OnFindTarget -= AggressiveBehaviour;
            base.OnDestroy();
        }
    }
}
