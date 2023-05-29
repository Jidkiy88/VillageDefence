using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerLabel;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color endColor;
        [SerializeField] private int duration;
        [SerializeField] private Vector3 hiddenPosition;

        public Action OnTimeOut;

        private Vector3 _startPosition;
        private int _currentSecond;
        private bool _painted;

        private void Start()
        {
            _startPosition = transform.position;
        }

        public void StartCountDown()
        {
            timerLabel.gameObject.SetActive(true);
            _currentSecond = duration;
            timerLabel.text = _currentSecond.ToString();
            timerLabel.faceColor = defaultColor;
            timerLabel.color = Color.white;
            _painted = false;
            StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            while (_currentSecond > 0)
            {
                if (_currentSecond < 4 && !_painted)
                {
                    timerLabel.DOColor(endColor, 3f);
                    _painted = true;
                }
                timerLabel.text = _currentSecond.ToString();
                yield return new WaitForSeconds(1);
                _currentSecond--;
            }
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(hiddenPosition, 0.5f)).AppendCallback(() =>
            {
                OnTimeOut?.Invoke();
                timerLabel.gameObject.SetActive(false);
                transform.position = _startPosition;
            });
        }
    }
}
