using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager singleton;
    private void CheckSingleton() {
        if (!singleton) {
            singleton = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this.gameObject);
        }
    }
    #endregion




    
}
