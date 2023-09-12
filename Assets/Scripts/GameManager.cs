using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;

namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;
        public NetworkVariable<int> CurrentTurn = new();
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

        public static string JoinCode;
        public async void StartHost()
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(1);

                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                var relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost(); 
                NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
            catch (RelayServiceException ex)
            {
                Debug.Log(ex);
            }
        }

        [SerializeField] private TMP_InputField _joinCodeInputField;
        public async void StartClient()
        {
            try
            {
                var allocation = await RelayService.Instance.JoinAllocationAsync(_joinCodeInputField.text);

                var relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException ex)
            {
                Debug.Log(ex);
            }
           
        }

        // Start is called before the first frame update
        async void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                Debug.Log("Connected:" + clientId);

                if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 1)
                {
                    SpawnBoard();
                }
            };
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        [SerializeField] private GameObject boardPrefab;
        private void SpawnBoard()
        {
            var newBoard = Instantiate(boardPrefab);
            newBoard.GetComponent<NetworkObject>().Spawn();
        }
    }
}
