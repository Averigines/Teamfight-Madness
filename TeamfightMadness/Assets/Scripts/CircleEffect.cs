using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CircleEffect : MonoBehaviour
{
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float effectDuration;

    private SpriteRenderer _renderer;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = startColor;
        var startTime = Time.time;
        _renderer.DOColor(endColor, effectDuration).OnComplete(() =>
        {
            var endTime = Time.time;
            print(endTime-startTime);
            Destroy(gameObject);
        });
    }
}
