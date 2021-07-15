using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRefill : MonoBehaviour
{
    [SerializeField] private float timeTillNewRefill;
    [SerializeField] private GameObject gunDisplay;
    private Collider triggerCollider;

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();
        gunDisplay.SetActive(true);
    }

    private IEnumerator C_RefillTimer()
    {
        yield return new WaitForSeconds(timeTillNewRefill);
        triggerCollider.enabled = true;
        gunDisplay.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 17)
        {
            triggerCollider.enabled = false;
            StartCoroutine(C_RefillTimer());
            gunDisplay.SetActive(false);
        }
        
    }
}
