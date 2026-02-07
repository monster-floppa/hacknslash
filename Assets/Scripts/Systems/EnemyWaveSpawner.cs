using UnityEngine;

namespace HacknSlash.Systems
{
    public class EnemyWaveSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class Wave
        {
            public GameObject[] Prefabs;
            public int Count = 3;
        }

        [SerializeField] private Wave[] waves;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float waveDelay = 2f;

        private int currentWave;
        private float timer;
        private int aliveEnemies;

        private void Start()
        {
            timer = waveDelay;
        }

        private void Update()
        {
            if (currentWave >= waves.Length)
            {
                return;
            }

            if (aliveEnemies > 0)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnWave(waves[currentWave]);
                currentWave++;
                timer = waveDelay;
            }
        }

        private void SpawnWave(Wave wave)
        {
            if (wave == null || wave.Prefabs == null || wave.Prefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
            {
                return;
            }

            int i;
            for (i = 0; i < wave.Count; i++)
            {
                GameObject prefab = wave.Prefabs[Random.Range(0, wave.Prefabs.Length)];
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject instance = (GameObject)Instantiate(prefab, point.position, point.rotation);
                aliveEnemies++;
                StartTracking(instance);
            }
        }

        private void StartTracking(GameObject enemy)
        {
            EnemyLifeTracker tracker = enemy.GetComponent<EnemyLifeTracker>();
            if (tracker == null)
            {
                tracker = enemy.AddComponent<EnemyLifeTracker>();
            }

            tracker.Initialize(this);
        }

        public void NotifyEnemyDead()
        {
            aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        }
    }

    public class EnemyLifeTracker : MonoBehaviour
    {
        private EnemyWaveSpawner owner;
        private HacknSlash.Combat.Hurtbox hurtbox;

        public void Initialize(EnemyWaveSpawner spawner)
        {
            owner = spawner;
            hurtbox = GetComponent<HacknSlash.Combat.Hurtbox>();
        }

        private void Update()
        {
            if (hurtbox != null && !hurtbox.IsAlive)
            {
                owner.NotifyEnemyDead();
                Destroy(this);
            }
        }
    }
}
