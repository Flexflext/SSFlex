using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grenade : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: Script for Bullet hitting Shit and Stuff

    [SerializeField] private GameObject explosion;
    [SerializeField] private float explosionTime;
    [SerializeField] private float explosionRadius;

    public System.Action<GameObject, float> HitAnything;

    // Start is called before the first frame update
    void Start()
    {
        // Timer for Explosion
        StartCoroutine(C_GrenadeExplosionTimer(explosionTime));
    }

    /// <summary>
    /// Enumerator to Destroy Bullet after Time and Spawn Explosion Effect
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator C_GrenadeExplosionTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        // Spawn Explosion Effect
        Instantiate(explosion, transform.position, Quaternion.identity);
        

        // Do Dmg
        Explode();

        // Destroy Only if photonView is mine
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
            PhotonNetwork.RemoveRPCs(photonView);
        }
        
    }

    /// <summary>
    /// Explode and Invoke Hit Event to do Dmg to Player
    /// </summary>
    private void Explode()
    {
        //Overlap Sphere to get all Colliders close
        Collider[] collider = Physics.OverlapSphere(transform.position, explosionRadius);

        // Check the Distance to 
        foreach (Collider col in collider)
        {
            float distance = (col.transform.position - transform.position).magnitude;
            float percent = 1-(distance / explosionRadius);

            // Invoke Hit Event
            if (HitAnything != null && distance <= explosionRadius)
            {
                HitAnything.Invoke(col.gameObject, percent);
            }
        }

        
    }

    /// <summary>
    /// Sync to Play Grenade Sound
    /// </summary>
    /// <param name="_audioName"></param>
    [PunRPC]
    public void SyncGrenade(string _audioName)
    {
        AudioManager.Instance.Play(_audioName);
    }
}
