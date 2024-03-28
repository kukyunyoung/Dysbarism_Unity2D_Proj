using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1NpcManager : MonoBehaviour
{
    [SerializeField] Npc Priest;
    [SerializeField] GameObject blessPanel;

    public void SetBless(string name)
    {
        blessPanel.SetActive(false);
        Priest.npcOn = false;
        UIManager.instance.stopTime--;
        PlayerMove.instance.SetBless(name);
    }
}
