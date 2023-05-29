using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace Scripts
{
    public class GameInit : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private int loadingDuration;
        [SerializeField] private Image progressBar;

        private void Awake()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            yield return new WaitForSeconds(1f);
            Sequence seq = DOTween.Sequence();
            seq.Append(progressBar.DOFillAmount(1f, loadingDuration))
                .AppendCallback(() => SceneManager.LoadScene(sceneName));
        }
    }
}
