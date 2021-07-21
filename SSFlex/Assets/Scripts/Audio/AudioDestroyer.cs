using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDestroyer());
    }

    private IEnumerator AutoDestroyer()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }
}
