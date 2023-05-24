using Plugins.MonoCache;
using UnityEngine;

namespace CarLogic
{
    public class TouchInput : MonoCache
    {
        [HideInInspector] public bool buttonPressed = false;
    
        private bool _changeScaleOnPressed = false;
        private RectTransform _rectTransform;
        private Vector3 _initialScale;
        private float _scaleDownMultiplier = 0.85f;

        void Start()
        {
            _rectTransform = Get<RectTransform>();
            _initialScale = _rectTransform.localScale;
        }

        public void ButtonDown()
        {
            buttonPressed = true;
        
            if (_changeScaleOnPressed) 
                _rectTransform.localScale = _initialScale * _scaleDownMultiplier;
        }

        public void ButtonUp()
        {
            buttonPressed = false;
        
            if (_changeScaleOnPressed) 
                _rectTransform.localScale = _initialScale;
        }
    }
}