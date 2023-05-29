using DG.Tweening;
using Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class DeathWindow : MonoBehaviour
    {
        [SerializeField] private PlayerBase player;

        [SerializeField] private Image backgroundImage;
        [SerializeField] private Transform loseLabel;
        [SerializeField] private Transform restartView;

        [SerializeField] private Color backgroundHideColor;
        [SerializeField] private Color backgroundShowColor;
        [SerializeField] private Vector3 loseLabelHidePos;
        [SerializeField] private Vector3 loseLabelShowPos;
        [SerializeField] private Vector3 restartViewHidePos;
        [SerializeField] private Vector3 restartViewShowPos;
        [SerializeField] private float animationDuration;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence();
            backgroundImage.color = backgroundHideColor;
            loseLabel.localPosition = loseLabelHidePos;
            restartView.localPosition = restartViewHidePos;
            seq.Append(backgroundImage.DOColor(backgroundShowColor, animationDuration))
                .Append(loseLabel.DOLocalMove(loseLabelShowPos, animationDuration))
                .Join(restartView.DOLocalMove(restartViewShowPos, animationDuration));
        }

        public void Close(Action callback)
        {
            backgroundImage.color = backgroundShowColor;
            loseLabel.localPosition = loseLabelShowPos;
            restartView.localPosition = restartViewShowPos;
            Sequence seq = DOTween.Sequence();
            seq.Append(loseLabel.DOLocalMove(loseLabelHidePos, animationDuration))
                .Join(restartView.DOLocalMove(restartViewHidePos, animationDuration))
                .Append(backgroundImage.DOColor(backgroundHideColor, animationDuration))
                .AppendCallback(() =>
                {
                    callback?.Invoke();
                    gameObject.SetActive(false);
                });
        }
    }
}
