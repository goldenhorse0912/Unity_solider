using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class Scroller : MonoBehaviour {
        bool _isScroll, _canScroll;
        bool _isScrollLeft;
        float _horizontalNormalizedPosition;

        [SerializeField]
        GameObject leftBtn, rightBtn;
        [SerializeField]
        float scrollSpeed = 0.1f;

        ScrollRect _scrollView;

        void Awake () {
            _scrollView = GetComponent<ScrollRect> ();
        }

        void OnEnable () {
            CheckChild ();
        }

        void Update () {
            if (!_canScroll) return;
            if (_isScroll) {
                if (_isScrollLeft) {
                    _horizontalNormalizedPosition = Mathf.Clamp (_scrollView.horizontalNormalizedPosition - Time.deltaTime * scrollSpeed, 0f, 1f);
                } else {
                    _horizontalNormalizedPosition = Mathf.Clamp (_scrollView.horizontalNormalizedPosition + Time.deltaTime * scrollSpeed, 0f, 1f);
                }
                _scrollView.horizontalNormalizedPosition = _horizontalNormalizedPosition;
            }
            if (_scrollView.horizontalNormalizedPosition >= .99f) {
                ScrollRight (false);
                rightBtn.Hide ();
            } else {
                rightBtn.Show ();
            }
            if (_scrollView.horizontalNormalizedPosition <= 0) {
                ScrollLeft (false);
                leftBtn.Hide ();
            } else {
                leftBtn.Show ();
            }
        }

        public void ScrollRight (bool state) {
            _isScroll = state;
            _isScrollLeft = false;
        }

        public void ScrollLeft (bool state) {
            _isScroll = state;
            _isScrollLeft = true;
        }

        [SerializeField]
        Transform container;

        void CheckChild () {
            var count = container.Cast<Transform> ().Count (child => child.gameObject.activeSelf);
            if (count > 2) {
                _canScroll = true;
                rightBtn.Show ();
                leftBtn.Show ();
            } else {
                _canScroll = false;
                rightBtn.Hide ();
                leftBtn.Hide ();
            }
        }
    }
}