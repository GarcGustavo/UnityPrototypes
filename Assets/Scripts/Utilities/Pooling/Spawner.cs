using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
	public PoolItem prefab;

	//TODO: Refactor into generic scripts

	[SerializeField] private PoolItem Prefab;

	[SerializeField] private BoxCollider SpawnArea;

	[SerializeField] private int BulletsPerSecond = 10;

	[SerializeField] private float Speed = 5f;

	[SerializeField] private bool UseObjectPool;

	private float LastSpawnTime;

	public ObjectPool<PoolItem> pool;
	//public delegate void OnDisableCallback(PoolItem Instance);

	private void Start()
	{
		pool = new ObjectPool<PoolItem>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, 200,
			100_000);
	}

	private void Update()
	{
		var delay = 1f / BulletsPerSecond;
		if (LastSpawnTime + delay < Time.time)
		{
			var bulletsToSpawnInFrame = Mathf.CeilToInt(Time.deltaTime / delay);
			while (bulletsToSpawnInFrame > 0)
			{
				if (!UseObjectPool)
				{
					var instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
					instance.transform.SetParent(transform, true);

					SpawnBullet(instance);
				}
				else
				{
					pool.Get();
				}

				bulletsToSpawnInFrame--;
			}

			LastSpawnTime = Time.time;
		}
	}

	private void OnGUI()
	{
		if (UseObjectPool)
		{
			GUI.Label(new Rect(10, 10, 200, 30), $"Total Pool Size: {pool.CountAll}");
			GUI.Label(new Rect(10, 30, 200, 30), $"Active Objects: {pool.CountActive}");
		}
	}

	private PoolItem CreatePooledObject()
	{
		var instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		//instance.Disable += ReturnObjectToPool;
		instance.gameObject.SetActive(false);

		return instance;
	}

	private void ReturnObjectToPool(PoolItem Instance)
	{
		pool.Release(Instance);
	}

	private void OnTakeFromPool(PoolItem Instance)
	{
		Instance.gameObject.SetActive(true);
		Instance.transform.SetParent(transform, true);
	}

	private void OnReturnToPool(PoolItem Instance)
	{
		Instance.gameObject.SetActive(false);
	}

	private void OnDestroyObject(PoolItem Instance)
	{
		//Instance.Disable -= ReturnObjectToPool;
		Destroy(Instance.gameObject);
	}

	private void SpawnBullet(PoolItem Instance)
	{
		var spawnLocation = new Vector3(
			SpawnArea.transform.position.x + SpawnArea.center.x +
			Random.Range(-1 * SpawnArea.bounds.extents.x, SpawnArea.bounds.extents.x),
			SpawnArea.transform.position.y + SpawnArea.center.y +
			Random.Range(-1 * SpawnArea.bounds.extents.y, SpawnArea.bounds.extents.y),
			SpawnArea.transform.position.z + SpawnArea.center.z +
			Random.Range(-1 * SpawnArea.bounds.extents.z, SpawnArea.bounds.extents.z)
		);

		Instance.transform.position = spawnLocation;

		Instance.Shoot(spawnLocation, SpawnArea.transform.right, Speed);
	}
}