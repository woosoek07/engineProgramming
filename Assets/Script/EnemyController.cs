using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float damage = 10f;
    public float attackInterval = 1f;
    public float hitShakePerDamage = 0.005f;
    public float killShake = 0.4f;

    private Rigidbody2D rig;
    private HPController hp;
    private Transform target;
    private float nextAttackTime;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        hp = GetComponent<HPController>();
    }

    public void Spawn(Vector2 pos, Transform player)
    {
        transform.position = pos;
        target = player;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 dir = ((Vector2)target.position - rig.position).normalized;
        rig.linearVelocity = dir * moveSpeed;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.transform != target || Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackInterval;

        HPController playerHP = target.GetComponent<HPController>();
        playerHP.TakeDamage(damage);
        
        if(playerHP.IsDead)
            target.gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        hp.TakeDamage(damage);
        CameraShake.Shake(damage * hitShakePerDamage, 0.1f);

        if(hp.IsDead)
        {
            CameraShake.Shake(killShake, 0.3f);
            gameObject.SetActive(false); // ¹ÝÈ¯
        }
    }
}
