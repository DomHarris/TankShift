using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float range = 1000;
    private RectTransform _rt;
    
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        _rt.anchoredPosition =  Vector2.right * Mathf.Sin(speed * Time.time) * range;
    }
}
