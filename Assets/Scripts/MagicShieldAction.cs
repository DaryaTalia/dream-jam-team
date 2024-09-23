using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShieldAction : MonoBehaviour
{
    [SerializeField]
    int magicDefense = 5;
    [SerializeField]
    int magicDuration = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MagicShield());
    }

    IEnumerator MagicShield()
    {
        GameManager.Instance.cargoController.Defense += magicDefense;
        yield return new WaitForSeconds(magicDuration);
        Destroy(gameObject);
        GameManager.Instance.cargoController.Defense -= magicDefense;
    }
}
