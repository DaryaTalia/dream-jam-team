using UnityEngine;

public class HealthPotionAction : MonoBehaviour
{
    [SerializeField] int healthAmount = 30;
    void Start()
    {
        AudioManager.instance.Play("Player Heal");
        GameManager.Instance.playerStats.AddHealth(healthAmount);
        Destroy(gameObject);
    }
}
