using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracePlayer : MonoBehaviour
{
    Fish parent;

    void Start()
    {
        parent = transform.parent.GetComponent<Fish>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player")) parent.isTrace = true;
    }
}
