using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.NPC
{
    public class CharactersAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void Death()
        {
            var randomNumber = Random.Range(0, 3);
            animator.SetInteger("DeathType", randomNumber);
            animator.SetTrigger("Death");
        }

        public void Revive()
        {
            animator.SetTrigger("Revive");
        }

        public void Attack()
        {
            var randomNumber = Random.Range(0, 3);
            animator.SetInteger("ArmedAttackType", randomNumber);
            animator.SetTrigger("Attack");
        }

        public void Idle()
        {
            animator.SetTrigger("Idle");
        }

        public void Cast()
        {
            animator.SetTrigger("Cast");
        }

        public void Scare(bool state)
        {
            animator.SetBool("Scared", state);
        }

        public void SetArmed(bool state)
        {
            animator.SetBool("IsArmed", state);
        }

        public void Hit()
        {
            var randomNumber = Random.Range(0, 4);
            animator.SetInteger("HitType", randomNumber);
            animator.SetTrigger("Hit");
        }

        public void Walk()
        {
            animator.SetTrigger("Walk");
        }

        public void Victory()
        {
            animator.SetTrigger("Victory");
        }
    }
}
