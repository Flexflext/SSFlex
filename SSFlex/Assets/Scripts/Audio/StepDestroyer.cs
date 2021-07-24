using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            StartCoroutine(AutoDestroyer());
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            StartCoroutine(AutoDestroyer());
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StartCoroutine(AutoDestroyer());
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StartCoroutine(AutoDestroyer());
        }
    }
    private IEnumerator AutoDestroyer()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}
