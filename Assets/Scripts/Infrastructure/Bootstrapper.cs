using Plugins.MonoCache;
using UnityEngine;

namespace Infrastructure
{
    public class Bootstrapper : MonoCache
    {
        [SerializeField] private LoadingCurtain _curtain;
        
        private Game _game;

        private void Awake()
        {
            _game = new Game(Instantiate(_curtain));
            _game.StateMachine.Enter<BootstrapState>();
            
            DontDestroyOnLoad(this);
        }
    }
}