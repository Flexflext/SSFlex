using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public System.Action<GameObject> OnHit;
    [SerializeField] private float timeTillDestroy = 4f;
    [SerializeField] private GameObject impactEffect;

    private void Start()
    {
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

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();
        Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (photonView.IsMine)
        {
            // Check if is not null
            if (OnHit != null)
            {
                OnHit.Invoke(collision.gameObject);
            }    

            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
