using System;
using System.Collections;
using System.Collections.Generic;
using Entity.Damage;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private Bounds _bounds;
    private Collider2D[] _results = new Collider2D[2];
    
    private void Awake()
    {
        _bounds = GetComponent<BoxCollider2D>().bounds;
    }

    private void FixedUpdate()
    {
        var num = Physics2D.OverlapBoxNonAlloc(transform.position, _bounds.size, 0, _results);
        for (int i = 0; i < num; i++)
        {
            var hits = _results[i].GetComponentsInChildren<IHitReceiver>();
            foreach (var hit in hits)
                hit.ReceiveHit(new HitData
                {
                    DamageType = DamageType.Effect,
                    Damage = 999
                });
        }
    }
}
