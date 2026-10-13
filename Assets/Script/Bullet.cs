using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;

    private Rigidbody2D rig;
    private float damage;
    private float despawnTime;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 pos, Vector2 dir, float speed, float damage)
    {
        transform.position = pos;
        this.damage = damage;
        despawnTime = Time.time + lifetime;
        gameObject.SetActive(true);
        rig.linearVelocity = dir * speed;
    }

    private void Update()
    {
        if(Time.time >= despawnTime)
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!gameObject.activeSelf || !col.TryGetComponent(out EnemyController enemy))
            return;
        enemy.TakeDamage(damage);
        gameObject.SetActive(false);
    }
}
