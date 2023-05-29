using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.NPC
{
    public class SkeletonBehaviour : NPCBase
    {
        protected override void Initialize()
        {
            base.Initialize();
            OnFindTarget += AggressiveBehaviour;
        }

        public override void Despawn()
        {
            base.Despawn();
            OnFindTarget -= AggressiveBehaviour;
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

        protected override void OnDestroy()
        {
            OnFindTarget -= AggressiveBehaviour;
            base.OnDestroy();
        }
    }
}
