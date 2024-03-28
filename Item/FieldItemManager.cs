using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItemManager : MonoBehaviour
{
    [SerializeField] GameObject[] prefab;
    public Inventory inventory;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)) 
        {
            int arr = Random.Range(0, prefab.Length);
            GameObject fieldItem = Instantiate(prefab[arr]);
            fieldItem.transform.parent = gameObject.transform;
        }
    }
}
