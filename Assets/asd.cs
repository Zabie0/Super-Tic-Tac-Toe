using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;
public class asd : MonoBehaviour
{
    public Button btn;
    public Image spr;
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(onclick);
        spr.enabled = false;
    }

    public void onclick(){
        spr.enabled = true;
        spr.color = new Color(0, 0, 255, 123);
        spr.sprite = Resources.Load<Sprite>("Circle");
    }
}