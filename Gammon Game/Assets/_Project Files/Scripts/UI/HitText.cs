using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitText : MonoBehaviour
{
    public float timeToDestroy = 1f;
    public float textTilt = 2f;
    public float spawnPosOffset = 0.5f;

    private void Start() {
        StartCoroutine(UnscaledDestroy(gameObject, timeToDestroy));
        transform.eulerAngles = new Vector3(-15,0,Random.Range(-textTilt, textTilt));
        transform.localPosition += new Vector3(Random.Range(-spawnPosOffset, spawnPosOffset), Random.Range(-spawnPosOffset, spawnPosOffset), 0);
    }

    private IEnumerator UnscaledDestroy(GameObject _gameObject, float _delay = 0) {
        yield return new WaitForSecondsRealtime(_delay);
        Destroy(_gameObject);
    }
}
