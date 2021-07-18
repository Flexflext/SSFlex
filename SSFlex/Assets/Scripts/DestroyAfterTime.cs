using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    [SerializeField] private bool destroyNetwork;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(C_TimeTillDestoy(destroyTime));
    }

    /// <summary>
    /// Time till Game Object gets Destroyed
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator C_TimeTillDestoy(float _time)
    {
        yield return new WaitForSeconds(_time);

        if (destroyNetwork)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
