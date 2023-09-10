using UnityEngine;
using SceneController = UnityEngine.SceneManagement.SceneManager;
namespace Assets.Scripts
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance;

        void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void NextLevel()
        {
            SceneController.LoadSceneAsync(SceneController.GetActiveScene().buildIndex + 1);
        }

        public void LoadScene(string sceneName)
        {
            SceneController.LoadSceneAsync(sceneName);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
