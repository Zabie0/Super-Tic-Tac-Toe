using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        // Start is called before the first frame update
        void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                Debug.Log("Connected:" + clientId);

                if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
                { 
                    SpawnBoard();
                }
            };
        }

        [SerializeField] private GameObject boardPrefab;
        private void SpawnBoard()
        {
            var newBoard = Instantiate(boardPrefab);
            newBoard.GetComponent<NetworkObject>().Spawn();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
