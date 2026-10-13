using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class BulletType
    {
        public Bullet prefab;
        public float damage = 10f;
        public float speed = 15f;
        public float fireInterval = 0.1f;
        public float recoil = 0f;

        [System.NonSerialized] public List<Bullet> pool = new List<Bullet>();
    }

    public float moveSpeed = 6f;
    public float recoilDamping = 8f;
    public float switchCooldown = 0.5f;
    public Transform look;

    public BulletType[] bulletTypes =
    {
        new BulletType{damage = 10f, speed = 18f, fireInterval = 0.1f, recoil = 0f},
        new BulletType{damage = 35f, speed = 11f , fireInterval = 0.6f, recoil = 5f }
    };

    private Rigidbody2D rig;
    private Camera camera;
    private Transform bulletParent;
    private Vector2 moveInput;
    private Vector2 Direction = Vector2.up;
    private Vector2 recoilVelocity;
    private int bulletIndex;
    private float nextFireTime;
    private float nextSwitchTime;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        camera = Camera.main;
        bulletParent = new GameObject("BulletPool").transform;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard == null || mouse == null)
            return;

        moveInput = 
            new Vector2((keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f), 
            (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f)).normalized;


        Vector2 mosueWorld = camera.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 toMouse = mosueWorld - rig.position;
        if (toMouse.sqrMagnitude > 0.0001f)
            Direction = toMouse.normalized;

        if (look != null)
            look.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);

        // ¿ìÅ¬¸¯: ÃÑ¾Ë º¯°æ
        if(mouse.rightButton.wasPressedThisFrame && Time.time >= nextSwitchTime)
        {
            bulletIndex = (bulletIndex + 1) % bulletTypes.Length;
            nextSwitchTime = Time.time + switchCooldown;
        }

        // ÁÂÅ¬¸¯: ÃÑ¾Ë ¹ß»ç
        if (mouse.leftButton.isPressed && Time.time >= nextFireTime)
            Fire();
    }

    private void FixedUpdate()
    {
        recoilVelocity = Vector2.Lerp(recoilVelocity, Vector2.zero, recoilDamping * Time.fixedDeltaTime);
        rig.linearVelocity = moveInput * moveSpeed + recoilVelocity;
        rig.rotation = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 90f;
    }

    private void Fire()
    {
        BulletType type = bulletTypes[bulletIndex];

        GetBullet(type).Fire(rig.position + Direction * 0.5f, Direction, type.speed, type.damage);

        nextFireTime = Time.time + type.fireInterval;
        recoilVelocity -= Direction * type.recoil; // ¹Ýµ¿
    }

    // Object Pooling
    private Bullet GetBullet(BulletType type)
    {
        foreach (Bullet bullet in type.pool)
            if (!bullet.gameObject.activeSelf)
                return bullet;

        Bullet newBullet = Instantiate(type.prefab, bulletParent);
        type.pool.Add(newBullet);
        return newBullet;
    }
}
