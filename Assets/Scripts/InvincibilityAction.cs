using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityAction : MonoBehaviour
{
    [SerializeField]
    int durationAmount = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InvincibilityCoroutine());
    }

    IEnumerator InvincibilityCoroutine()
    {
        GameManager.Instance.playerStats.ToggleInvincibility(); // Turn On
        AudioManager.instance.Play("Player Heal");
        yield return new WaitForSeconds(durationAmount);
        GameManager.Instance.playerStats.ToggleInvincibility(); // Turn Off
        Destroy(gameObject);
    }

}
