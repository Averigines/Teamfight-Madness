using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameModel;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private SpriteRenderer _healthBar;
    private int _maxHealth;
    private float _scalingPerHealthPoint;

    private const float OffsetY = -0.2f;

    [SerializeField] private Color fullHealthColor;
    [SerializeField] private Color noHealthColor;

    // Start is called before the first frame update
    void Awake()
    {
        _healthBar = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int maxHealth, SpriteRenderer parentRenderer)
    {
        float parentHeight = parentRenderer.bounds.size.y;
        transform.localPosition = new Vector3(transform.localPosition.x, -(parentHeight / 2) + OffsetY, transform.localPosition.z);
        _maxHealth = maxHealth;
        _scalingPerHealthPoint = transform.localScale.x / _maxHealth;
        _healthBar.color = fullHealthColor;
    }

    public void UpdateHealthBar(int currHealth)
    {
        float newScale = _scalingPerHealthPoint * currHealth;
        float healthNormalized = (float)currHealth / _maxHealth;
        Color newColor = Color.Lerp(noHealthColor, fullHealthColor, healthNormalized);

        Sequence animSequence = DOTween.Sequence();
        Tweener scaleTween = transform.DOScaleX(newScale, 0.2f).SetEase(Ease.OutSine);
        Tweener colorTween = _healthBar.DOColor(newColor, 0.2f).SetEase(Ease.OutSine);

    }
}
