using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.NPC
{
    public class ParticlesHolder : MonoBehaviour
    {
        [SerializeField] private ParticleSystem healParticle;
        [SerializeField] private ParticleSystem castParticle;
        [SerializeField] private ParticleSystem hitParticle;

        public void OnHit()
        {
            hitParticle.Play();
        }

        public void OnHeal()
        {
            healParticle.Play();
        }

        public void OnCast()
        {
            castParticle.Play();
        }
    }
}
