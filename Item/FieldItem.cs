using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    [SerializeField] ItemData itemData;
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject instPrefab;

    bool isPick;
    float dirX;
    float dirY;
    Vector2 startPos;
    Vector2 endPos;

    void Start()
    {
        dirX = Random.Range(-2f, 2f);
        dirY = Random.Range(0.5f, 2f);
        startPos = transform.position;
        endPos = startPos + new Vector2(dirX, dirY);

        inventory = UIManager.instance.transform.GetChild(5).GetComponent<Inventory>();

        StartCoroutine(ItemRoutine());
    }

    IEnumerator ItemRoutine()
    {
        float startTime = Time.time;
        float duration = 0.6f;

        while (true)
        {
            transform.position = Vector3.Lerp(startPos, endPos, Easing.SingleAxisBezier.CubicBezier(Easing.Preset.FastInSlowOut, Time.time - startTime, duration));

            if (Time.time - startTime > duration)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(!isPick && other.transform.CompareTag("Player"))    
        {
            isPick = true;
            inventory.Add(itemData);
            Destroy(gameObject,0.3f);
        }
    }
}
