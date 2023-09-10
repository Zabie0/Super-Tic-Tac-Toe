using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            SceneManager.Instance.NextLevel();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            SceneManager.Instance.NextLevel();
        }

        // Start is called before the first frame update
        void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                Debug.Log("Connected" + clientId);
            };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
