using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnChance;
    }

    public SpawnableObject[] objects;
    public SpawnableObject scoreZone;
    public float minSpawnRate = 0.7f;
    public float maxSpawnRate = 3f;
    public float spawnRate;

    private void OnEnable()
    {
        spawnRate = maxSpawnRate;
        Invoke(nameof(Spawn), (spawnRate < minSpawnRate ? minSpawnRate : spawnRate));
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Spawn()
    {
        float spawnChance = Random.value;

        foreach (SpawnableObject obj in objects)
        {
            if (spawnChance < obj.spawnChance)
            {
                GameObject obstacle = Instantiate(obj.prefab);
                obstacle.transform.position += transform.position;

                break;
            }

            spawnChance -= obj.spawnChance;
        }


        Invoke(nameof(Spawn), (spawnRate < minSpawnRate ? minSpawnRate : spawnRate));
    }

}
