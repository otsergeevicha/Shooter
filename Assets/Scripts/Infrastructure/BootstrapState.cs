namespace Infrastructure
{
    public class BootstrapState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;

            RegisterServices();
        }

        public void Enter()
        {
            _sceneLoader.LoadScene(Constants.InitialScene, EnterLoadLevel);
        }

        private void EnterLoadLevel()
        {
            _stateMachine.Enter<LoadLevelState, string>("OpenCity");
        }

        public void Exit()
        {
        }

        private void RegisterServices()
        {
            ServiceRouter.Container.RegisterSingle<IAssetsProvider>(new AssetsProvider());
            ServiceRouter.Container.RegisterSingle<IGameFactory>(new GameFactory(ServiceRouter.Container.Single<IAssetsProvider>()));
        }
    }
}