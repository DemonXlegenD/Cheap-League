using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Instance statique de GameManager
    private static GameManager _instance;

    // Propriété pour accéder à l'instance
    public static GameManager Instance
    {
        get
        {
            // Si l'instance est nulle, cherchez dans la scène une instance existante
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                // Si aucune instance n'existe dans la scène, créez-en une
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void StartGame()
    {
        Debug.Log("Le jeu commence !");
    }

    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}