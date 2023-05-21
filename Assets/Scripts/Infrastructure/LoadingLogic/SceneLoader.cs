using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.LoadingLogic
{
    public class SceneLoader
    {
        public void LoadScene(string nextScene, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == nextScene)
            {
                onLoaded?.Invoke();
                return;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);
            waitNextScene.completed += _ => 
                onLoaded?.Invoke();
        }
    }
}