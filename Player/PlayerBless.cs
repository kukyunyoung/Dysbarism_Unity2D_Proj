using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dtd

public class PlayerBless : MonoBehaviour
{
    [SerializeField] GameObject[] bless; // att, heal, move

    GameObject selectedBless;
    Vector3 upPos = new Vector3(0, 1, 0);
    
    public void SelectBelss(string bless)
    {
        Destroy(selectedBless);
        switch (bless)
        {
            case "Att":
                selectedBless = Instantiate(this.bless[0]);
                break;
            case "Heal":
                selectedBless = Instantiate(this.bless[1]);
                break;
            case "Move":
                selectedBless = Instantiate(this.bless[2]);
                break;
            default:
                print("Bless Not Set");
                break;
        }
    }
}
