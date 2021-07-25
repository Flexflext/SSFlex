using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRefill : MonoBehaviour
{
    // Script von Felix
    // Purpose: Weapon Refill

    [SerializeField] private float timeTillNewRefill;
    [SerializeField] private GameObject gunDisplay;
    private Collider triggerCollider;

    private void Start()
    {
        // Get Components
        triggerCollider = GetComponent<Collider>();
        gunDisplay.SetActive(true);
    }

    /// <summary>
    /// Wait Time to Respawn
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_RefillTimer()
    {
        yield return new WaitForSeconds(timeTillNewRefill);
        triggerCollider.enabled = true;
        gunDisplay.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if Collided with Player
        if (other.gameObject.layer == 17)
        {
            triggerCollider.enabled = false;
            StartCoroutine(C_RefillTimer());
            gunDisplay.SetActive(false);
        }
        
    }
}
