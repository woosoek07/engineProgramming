using System.Collections.Generic;
using UnityEngine;

// 빈 오브젝트에 붙인다. 적을 미리 만들어 두고(Pooling) 일정 간격으로 대열을 골라 소환
public class EnemyManager : MonoBehaviour
{
    public EnemyController enemyPrefab;
    public Transform player;
    public int poolSize = 50;
    public float waveInterval = 4f;
    public int enemiesPerWave = 8;
    public float spacing = 0.8f;
    public float circleRadius = 8f;

    private readonly List<EnemyController> pool = new List<EnemyController>();
    private float nextWaveTime;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            EnemyController enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);
            pool.Add(enemy);
        }
    }

    private void Update()
    {
        if (Time.time < nextWaveTime || !player.gameObject.activeSelf) return;

        nextWaveTime = Time.time + waveInterval;
        SpawnWave(Random.Range(0, 5));
    }

    private void SpawnWave(int formation)
    {
        // 화면 밖 한 점을 대열 중심으로, 플레이어 쪽을 "앞"으로 회전
        Vector2 center = GetRandomPositionOutside();
        Vector2 toPlayer = (Vector2)player.position - center;
        Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg);

        for (int i = 0; i < enemiesPerWave; i++)
        {
            EnemyController enemy = GetEnemy();
            if (enemy == null) return; // 풀이 모두 사용 중

            enemy.Spawn(GetPosition(formation, i, center, rotation), player);
        }
    }

    // 대열 모양: 로컬 좌표(+X가 앞)를 만든 뒤 회전시켜 중심에 더한다
    private Vector2 GetPosition(int formation, int i, Vector2 center, Quaternion rotation)
    {
        switch (formation)
        {
            case 0: // 횡대
                return center + (Vector2)(rotation * new Vector2(0f, (i - (enemiesPerWave - 1) * 0.5f) * spacing));

            case 1: // V자
                int rank = (i + 1) / 2;
                float side = (i % 2 == 0) ? -1f : 1f;
                return center + (Vector2)(rotation * new Vector2(-rank, side * rank) * spacing);

            case 2: // 사각 밀집 (3열)
                return center + (Vector2)(rotation * new Vector2(-(i / 3), i % 3 - 1) * spacing);

            case 3: // 포위: 플레이어 주변 원형
                float angle = i * 2f * Mathf.PI / enemiesPerWave;
                return (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;

            default: // 산개: 화면 밖 아무 곳
                return GetRandomPositionOutside();
        }
    }

    // 카메라 화면 테두리보다 1칸 바깥의 무작위 위치
    private Vector2 GetRandomPositionOutside()
    {
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize + 1f;
        float halfWidth = cam.orthographicSize * cam.aspect + 1f;
        Vector2 camPos = cam.transform.position;

        if (Random.value < 0.5f) // 왼쪽 또는 오른쪽
            return camPos + new Vector2(Random.value < 0.5f ? -halfWidth : halfWidth, Random.Range(-halfHeight, halfHeight));

        // 위쪽 또는 아래쪽
        return camPos + new Vector2(Random.Range(-halfWidth, halfWidth), Random.value < 0.5f ? -halfHeight : halfHeight);
    }

    private EnemyController GetEnemy()
    {
        foreach (EnemyController enemy in pool)
        {
            if (!enemy.gameObject.activeSelf) return enemy;
        }
        return null;
    }
}