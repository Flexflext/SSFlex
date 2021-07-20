using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grenade : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float explosionTime;
    [SerializeField] private float explosionRadius;

    public System.Action<GameObject, float> HitAnything;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(C_GrenadeExplosionTimer(explosionTime));
    }

    private IEnumerator C_GrenadeExplosionTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        Instantiate(explosion, transform.position, Quaternion.identity);
        

        // Do Dmg
        Explode();

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
            PhotonNetwork.RemoveRPCs(photonView);
        }
        
    }


    private void Explode()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in collider)
        {
            float distance = (transform.position - col.transform.position).sqrMagnitude;
            float percent = distance / explosionRadius;

            if (HitAnything != null)
            {
                HitAnything.Invoke(col.gameObject, percent);
            }
        }

        
    }

    [PunRPC]
    public void SyncGrenade(string _audioName)
    {
        AudioManager.Instance.Play(_audioName);
    }
}
