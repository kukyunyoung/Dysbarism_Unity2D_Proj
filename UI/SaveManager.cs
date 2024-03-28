using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

[System.Serializable]
public class SaveData{
    public List<ItemData> invenData = new List<ItemData>(); // 인벤토리 데이터
    public List<float> playerStatusData = new List<float>(); // 플레이어 스탯 데이터

    public float playTime;
    public int gold;
    public int playerHp;
    public int playerHpMax;
    // 맵 진행도
    // 플레이어 음식
}

public class SaveManager : MonoBehaviour
{
    string path;
    Inventory inventory;
    PlayerStatus ps;

    bool isLoading;
    bool isSaving;

    void Start()
    {
        inventory = transform.GetChild(5).GetComponent<Inventory>();
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
        path = Path.Combine(Application.dataPath, "database.json");
        //JsonLoad();
    }

    void Update()
    {
        if (!isSaving && Input.GetKeyDown(KeyCode.F12)) JsonSave();
        if (!isLoading && Input.GetKeyDown(KeyCode.F10)) JsonLoad();
    }

    public void JsonLoad()
    {
        isLoading = true;
        SaveData saveData = new SaveData();

        if(!File.Exists(path))
        {
            UIManager.coin = 20;
            PlayerHP.hP = 200;
            PlayerHP.hp_Max = 200;
            //JsonSave();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            if(saveData != null)
            {
                inventory.PutItemList(saveData.invenData);
                ps.playerStatus.Clear();
                for (int i = 0; i < saveData.playerStatusData.Count; i++)
                {
                    ps.playerStatus.Add(saveData.playerStatusData[i]);
                }
                UIManager.coin = saveData.gold;
                UIManager.playTime = saveData.playTime;
                PlayerHP.hP = saveData.playerHp;
                PlayerHP.hp_Max = saveData.playerHpMax;
            }
        }
        isLoading = false;
        UIManager.instance.SetSystemMessage("로딩 완료!");
    }

    // 골드를 제외한 모든 플레이정보 초기화
    public void InitJson()
    {
        isLoading = true;
        SaveData saveData = new SaveData();

        /*if (!File.Exists(path))
        {
            UIManager.coin = 20;
            PlayerHP.hP = 200;
            PlayerHP.hp_Max = 200;
            JsonSave();
        }
        else
        {
            string loadJson = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            inventory.RemoveAll();
            ps.InitStatus();
            UIManager.coin = saveData.gold;
            UIManager.playTime = 0;
            PlayerHP.hP = 200;
            PlayerHP.hp_Max = 200;
            JsonSave();
        }*/

        inventory.RemoveAll();
        ps.InitStatus();
        UIManager.coin = 20;
        UIManager.playTime = 0;
        PlayerHP.hP = 200;
        PlayerHP.hp_Max = 200;

        isLoading = false;
        print("저장완료");
        
    }

    public void JsonSave()
    {
        isSaving = true;
        SaveData saveData = new SaveData();

        for(int i=0; i<inventory.Capacity; i++)
        {
            saveData.invenData.Add(inventory.GetItemData(i));
        }
        for(int i=0;i<ps.playerStatus.Count; i++)
        {
            saveData.playerStatusData.Add(ps.playerStatus[i]);
        }
        saveData.playTime = Time.time;
        saveData.gold = UIManager.coin;
        saveData.playerHp = PlayerHP.hP;
        saveData.playerHpMax = PlayerHP.hp_Max;

        string json = JsonUtility.ToJson(saveData); // string json = JsonUtility.ToJson(saveData, true) true옵션 빠지면 직렬화

        File.WriteAllText(path, json);
        isSaving = false;
        UIManager.instance.SetSystemMessage("저장 완료!");
    }
}
