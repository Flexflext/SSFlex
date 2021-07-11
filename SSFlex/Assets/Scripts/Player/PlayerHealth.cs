using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float shield;
    [SerializeField] private float timeUntilShieldRegen;
    [SerializeField] private float regenPerSecond;
    private float maxHealth;
    private float maxShield;
    private float currentTime;

    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        maxShield = shield;
    }

    // Update is called once per frame
    void Update()
    {
        if (shield <= maxShield && !dead)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                shield += Time.deltaTime * regenPerSecond;
            }
        }
    }

    public void TakeDamage(float _dmg)
    {
        float difference = shield - _dmg;
        currentTime = timeUntilShieldRegen;

        if (difference >= 0)
        {
            shield -= _dmg;
        }
        else
        {
            shield = 0f;
            health += difference;
        }

        if (health <= 0)
        {
            Debug.Log("I am Dead");
            dead = true;
        }
    }
}
