using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoController : MonoBehaviour
{
    [SerializeField]
    float _health;
    const float _maxHealth = 100;
    public float Health {
        get => _health;
        set
        {
            if(value > _maxHealth)
            {
                _health = _maxHealth;
            } else if(value < 0)
            {
                _health = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Health = _maxHealth;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
