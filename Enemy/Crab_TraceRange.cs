using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab_TraceRange : MonoBehaviour
{
    Crab parent;
    bool isRange;

    void Start()
    {
        parent = transform.parent.GetChild(0).GetComponent<Crab>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player") && !isRange)
        {
            isRange = true;
            parent.crabState = Crab.CrabState.idle;
            parent.anim.SetBool("Guard", false);
            StartCoroutine(parent.ChoiceAttack());
        }
    }
}
