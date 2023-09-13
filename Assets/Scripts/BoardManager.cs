using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class BoardManager : NetworkBehaviour
    {
        private Button[,] _buttons = new Button[9, 9];
        [SerializeField] private TextMeshProUGUI _joinCodeText;
        int [,] win = { {0,1,2}, {3,4,5}, {6,7,8}, {0,3,6}, {1,4,7}, {2,5,8}, {0,4,8}, {2,4,6} };
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
            CheckPanelForWin(r);
        }

        private void CheckPanelForWin(int panelIndex)
        {
            List<Sprite> panelSprites = new List<Sprite>();
            var panel = gameObject.transform.GetChild(0).GetChild(panelIndex).gameObject;
            var buttons = panel.GetComponentsInChildren<Image>();
            for(int j = 1; j < 10; j++)
            {
                panelSprites.Add(buttons[j].sprite);
            }
            for(int i = 0; i < win.Length/3; i++)
            {
                if(panelSprites[win[i,0]].name != "Background" && panelSprites[win[i,0]].name == panelSprites[win[i, 1]].name && panelSprites[win[i, 1]].name == panelSprites[win[i, 2]].name)
                {
                    for(int a = 1; a < buttons.Length; a++)
                    {
                        Destroy(buttons[a].gameObject);
                    }
                    
                    if(GameManager.Instance.CurrentTurn.Value == 1)
                    {
                        panel.GetComponent<Image>().sprite = xSprite;
                        ChangePanelSpriteClientRpc(panelIndex);
                    } else 
                    {
                        panel.GetComponent<Image>().sprite = oSprite;
                        ChangePanelSpriteServerRpc(panelIndex);
                    }
                    break;
                }
            }
        }

        [ClientRpc]
        private void ChangePanelSpriteClientRpc(int panelIndex){
            gameObject.transform.GetChild(0).GetChild(panelIndex).GetComponent<Image>().sprite = xSprite;
        }
        [ServerRpc]
        private void ChangePanelSpriteServerRpc(int panelIndex){
            gameObject.transform.GetChild(0).GetChild(panelIndex).GetComponent<Image>().sprite = oSprite;
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
