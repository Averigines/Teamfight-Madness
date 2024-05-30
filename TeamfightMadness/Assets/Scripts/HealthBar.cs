using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private int _maxHealth;
    private float _scalingPerHealthPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int maxHealth)
    {
        _maxHealth = maxHealth;
        _scalingPerHealthPoint = transform.localScale.x / _maxHealth;
    }

    public void UpdateHealthBar(int currHealth)
    {
        Vector3 newScale = new Vector3(_scalingPerHealthPoint * currHealth, transform.localScale.y);
        transform.localScale = newScale;
    }
}
