using SManager = UnityEngine.SceneManagement.SceneManager;

public enum SceneName
{
    Intro,
    Game
}

public class SceneManager
{
    private static SceneManager _instance = new ();
    public static SceneManager Instance => _instance;

    public void LoadScene(SceneName sceneName)
    {
        SManager.LoadSceneAsync(sceneName.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
