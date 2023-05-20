using Plugins.MonoCache;
using UnityEngine;

namespace Infrastructure
{
    public class GameRunner : MonoCache
    {
        [SerializeField] private Bootstrapper _bootstrapper;

        private void Awake()
        {
            var bootstrapper = FindObjectOfType<Bootstrapper>();

            if (bootstrapper != null)
                return;

            Instantiate(_bootstrapper);
        }
    }
}