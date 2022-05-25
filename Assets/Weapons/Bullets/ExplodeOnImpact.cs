using System;
using System.Collections;
using System.Collections.Generic;
using Bullets;
using Entity.Damage;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BulletImpact))]
public class ExplodeOnImpact : MonoBehaviour
{
    private static Dictionary<ParticleSystem, ParticleSystem> _particleSystems = new Dictionary<ParticleSystem, ParticleSystem>();

    [SerializeField] private Transform debris;
    [SerializeField] private ParticleSystem explosionPrefab;
    
    
    private BulletImpact _impact;

    private void Awake()
    {
        _impact = GetComponent<BulletImpact>();
        if (!_particleSystems.ContainsKey(explosionPrefab))
            _particleSystems.Add(explosionPrefab, Instantiate(explosionPrefab));
    }

    private void OnEnable()
    {
        _impact.Hit += ImpactOnHit;
    }

    private void OnDisable()
    {
        _impact.Hit -= ImpactOnHit;
    }
    
    private void ImpactOnHit(HitData obj)
    {
        var emit = new ParticleSystem.EmitParams
        {
            position = transform.position
        };
        _particleSystems[explosionPrefab].Emit(emit, 1);
        LeanPool.Spawn(debris, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }

}
