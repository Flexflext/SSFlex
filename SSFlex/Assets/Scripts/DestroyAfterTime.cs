using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyAfterTime : MonoBehaviour
{
    // Script von Felix
    // Purpose: Leben vom Spieler + VFX Play

    [SerializeField] private float destroyTime;
    [SerializeField] private bool destroyNetwork;

    // Start is called before the first frame update
    void Start()
    {
        // Start the Destroy Coroutine
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

        // Check if the Obj should be destroyed by Network
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
