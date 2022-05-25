using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitText : MonoBehaviour
{
    public float timeToDestroy = 1f;
    public float textTilt = 2f;

    private void Start() {
        Destroy(gameObject, timeToDestroy);
        //GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 1);
        transform.eulerAngles = new Vector3(0,0,Random.Range(-textTilt, textTilt));
    }
}
