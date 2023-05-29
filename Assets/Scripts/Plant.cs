using Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Plant : MonoBehaviour
    {
        [SerializeField] private PlayerBase player;
        [SerializeField] List<GameObject> crops;
        [SerializeField] private InteractableObject interactor;
        [SerializeField] private int growDuration;
        [SerializeField] private float healthGain;

        private bool _readyToHarvest = false;

        private void Start()
        {
            interactor.OnInteract += HarvestCrops;
            StartCoroutine(GrowCrops());
        }

        private void OnDestroy()
        {
            interactor.OnInteract -= HarvestCrops;
        }

        private void HarvestCrops()
        {
            if (!_readyToHarvest)
            {
                return;
            }

            crops.ForEach(c => c.gameObject.SetActive(false));
            player.GetHeal(healthGain);
            _readyToHarvest = false;
            StartCoroutine(GrowCrops());
        }

        private IEnumerator GrowCrops()
        {
            yield return new WaitForSeconds(growDuration);
            crops.ForEach(c => c.gameObject.SetActive(true));
            _readyToHarvest = true;
        }
    }
}
