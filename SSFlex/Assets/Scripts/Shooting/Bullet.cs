using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: Script for Bullet hitting Shit and Stuff

    public System.Action<GameObject> OnHit;
    [SerializeField] private float timeTillDestroy = 4f;
    [SerializeField] private GameObject impactEffect;

    private bool canDmgAgain = true;

    private void Start()
    {
        // Start Coroutine to Destroy Bullet after some Time
        StartCoroutine(C_TimeTillDestoy(timeTillDestroy));
    }

    /// <summary>
    /// Time till Bullet gets Destroyed
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator C_TimeTillDestoy(float _time)
    {
        yield return new WaitForSeconds(_time);

        // Destroy Bullet after Time
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Play Impact Hit on Copllision and Stops all Coroutines of Obj


        StopAllCoroutines();
        Instantiate(impactEffect, transform.position, Quaternion.identity);


        // Check if is not null
        if (OnHit != null)
        {
            //canDmgAgain = false;
            // BulletHitEvent
            OnHit.Invoke(collision.gameObject);
        }

        // Destroy after Hit
        Destroy(this.gameObject);
    }
}
