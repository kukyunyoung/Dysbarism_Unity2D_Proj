using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPullingManager : MonoBehaviour
{
    public static ObjPullingManager instance;

    private void Awake() 
    {
        #region SingleTon
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
        #endregion SingleTon    
    }
}
