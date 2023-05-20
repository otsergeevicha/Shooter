namespace Infrastructure
{
    public class Game
    {
        public readonly GameStateMachine StateMachine;

        public Game(LoadingCurtain curtain) => 
            StateMachine = new GameStateMachine(new SceneLoader(), curtain);
    }
}