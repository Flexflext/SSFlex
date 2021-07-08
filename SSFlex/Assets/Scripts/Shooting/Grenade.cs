using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float explosionTime;
    [SerializeField] private float explosionRadius;

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
        Destroy(this.gameObject);
    }
}
