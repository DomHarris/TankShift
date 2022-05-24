using System;
using System.Collections;
using System.Collections.Generic;
using Entity.Stats;
using UnityEngine;

public class TEST_UpdateStatInsideTrigger : MonoBehaviour
{
    [SerializeField] private StatType statToUpdate;
    [SerializeField] private float modifier = 0.5f;
    [SerializeField] private float range;

    private List<StatController> _initialValues = new List<StatController>();
    private StatModifier _modifier;

    private void Awake()
    {
        _modifier = new StatModifier
        {
            Type = statToUpdate,
            Value = modifier
        };
    }

    private void FixedUpdate()
    {
        var collided = Physics2D.OverlapCircle(transform.position, range)?.GetComponent<StatController>();
        if (collided == null)
        {
            foreach (var controller in _initialValues)
                controller.RemoveModifier(_modifier);
            _initialValues.Clear();   
        }
        else
        {
            if (!_initialValues.Contains(collided))
            {
                _initialValues.Add(collided);
                collided.AddModifier(_modifier);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
