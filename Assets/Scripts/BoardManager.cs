using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using TMPro;

namespace Assets.Scripts
{
    public class BoardManager : NetworkBehaviour
    {
        private Button[,] _buttons = new Button[9, 9];
        [SerializeField] private TextMeshProUGUI _joinCodeText;
        public override void OnNetworkSpawn()
        {
            _joinCodeText.text = GameManager.JoinCode;
            var cells = gameObject.transform.GetChild(0).GetComponentsInChildren<GridLayoutGroup>();
            for (int i = 1; i < 10; i++)
            {
                var buttons = cells[i].GetComponentsInChildren<Button>();
                int index = i - 1;
                for (int j = 0; j < 9; j++)
                {
                    _buttons[index, j] = buttons[j];

                    int r = index;
                    int c = j;

                    _buttons[index, j].onClick.AddListener(delegate
                    {
                        OnClickCell(r, c);
                    });
                }
            }
        }

        [SerializeField] private Sprite xSprite, oSprite;
        private void OnClickCell(int r, int c)
        {
            if (NetworkManager.Singleton.IsHost && GameManager.Instance.CurrentTurn.Value == 0)
            {
                _buttons[r, c].GetComponent<Image>().sprite = xSprite;
                _buttons[r, c].interactable = false;
                ChangeSpriteClientRpc(r, c);
                GameManager.Instance.CurrentTurn.Value = 1;
            }
            else if (!NetworkManager.Singleton.IsHost && GameManager.Instance.CurrentTurn.Value == 1)
            {
                _buttons[r, c].GetComponent<Image>().sprite = oSprite;
                _buttons[r, c].interactable = false;
                ChangeSpriteServerRpc(r, c);
            }

        }

        [ClientRpc]
        private void ChangeSpriteClientRpc(int r, int c)
        {
            _buttons[r, c].GetComponent<Image>().sprite = xSprite;
            _buttons[r, c].interactable = false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeSpriteServerRpc(int r, int c)
        {
            _buttons[r, c].GetComponent<Image>().sprite = oSprite;
            _buttons[r, c].interactable = false;
            GameManager.Instance.CurrentTurn.Value = 0;
        }
    }
}
