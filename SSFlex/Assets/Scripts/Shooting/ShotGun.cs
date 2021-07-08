using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    [SerializeField] private int pelltAmount;

    protected override void FireBullet()
    {
        for (int i = 0; i < pelltAmount; i++)
        {
            base.FireBullet();
        }
    }
}
