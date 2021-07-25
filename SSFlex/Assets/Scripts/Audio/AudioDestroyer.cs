using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDestroyer : MonoBehaviour
{
    // Code: Haoke
    // Responsible for:
    // - Destroying instantiated sounds afer 3 seconds.

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
