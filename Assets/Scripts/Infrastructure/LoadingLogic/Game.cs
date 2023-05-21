using Infrastructure.GameAI.StateMachine;
using Infrastructure.LoadingLogic.ScreenLoading;

namespace Infrastructure.LoadingLogic
{
    public class Game
    {
        public readonly GameStateMachine StateMachine;

        public Game(LoadingCurtain curtain) => 
            StateMachine = new GameStateMachine(new SceneLoader(), curtain);
    }
}