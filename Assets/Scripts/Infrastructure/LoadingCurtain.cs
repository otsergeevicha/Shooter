using Cysharp.Threading.Tasks;
using Plugins.MonoCache;
using UnityEngine;

namespace Infrastructure
{
    public class LoadingCurtain : MonoCache
    {
        [SerializeField] private CanvasGroup _curtain;

        private void Awake() =>
            DontDestroyOnLoad(this);

        public void Show()
        {
            gameObject.SetActive(true);
            _curtain.alpha = 1;
        }

        public void Hide() => 
            FadeIn();

        private async void FadeIn()
        {
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= Constants.RateAlfaCurtain;
                await UniTask.Delay(Constants.RateCurtain);
            }

            gameObject.SetActive(false);
        }
    }
}