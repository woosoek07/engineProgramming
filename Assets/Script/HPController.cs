using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    public float maxHP = 100f;
    public Slider hpSlider;

    private float hp;

    public bool IsDead => hp <= 0f;

    private void OnEnable()
    {
        hp = maxHP;
        UpdateBar();
    }

    public void TakeDamage(float damage)
    {
        hp = Mathf.Max(0f, hp - damage);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (hpSlider != null)
            hpSlider.value = hp / maxHP;
    }

}
