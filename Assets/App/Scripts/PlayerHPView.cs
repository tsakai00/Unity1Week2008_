using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHPView : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _playerHPTextList;

    public void UpdateHP(int hp)
    {
        for(int i = 0; i < 3; i++)
        {
            var txt = _playerHPTextList[i];
            txt.text = (hp > i) ? "▲" : "-";
        }
    }
}
