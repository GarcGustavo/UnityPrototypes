using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Pool;

public class Weapon : MonoBehaviour
{
	public Bullet prefab;

	[SerializeField] private float fireRate;
	[SerializeField] private float bulletSpeed;
	[SerializeField] private bool canShoot;
	[SerializeField] private MMFeedbacks weaponFeedbacks;
	public ObjectPool<Bullet> pool;

	private void Awake()
	{
		//pool = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, 100, 100);
		pool = new ObjectPool<Bullet>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject);

		for (var i = 0; i < 100; i++) CreatePooledObject();

		//Subscribing to input events
		InputController.Shoot += FireWeapon;
		Bullet.Disable += pool.Release;
	}

	private void Start()
	{
		canShoot = true;
	}

	private void FireWeapon(bool state, Vector3 direction)
	{
		//FIX: getting same object from pool, need to move create object into if statement here possibly
		if (canShoot)
		{
			var instance = pool.Get();
			//pool.Release(instance);
			weaponFeedbacks?.PlayFeedbacks();
			//instance.SetDirection(direction);
			StartCoroutine(BulletCD(1f / fireRate));
		}
	}

	private IEnumerator BulletCD(float delay)
	{
		canShoot = false;
		yield return new WaitForSeconds(delay);
		canShoot = true;
	}

	//////////////////////////////////////////////////////////
	//---------- Object Pooling callback functions ---------//
	//////////////////////////////////////////////////////////

	private Bullet CreatePooledObject()
	{
		var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
		obj.gameObject.SetActive(false);
		return obj;
	}

	private void OnTakeFromPool(Bullet obj)
	{
		obj.gameObject.SetActive(true);
		obj.transform.position = transform.position;
		obj.SetDirection(transform.forward, bulletSpeed);
	}

	private void OnReturnToPool(Bullet obj)
	{
		Debug.Log("Returned to pool! (If this happens object might be colliding w/ player)");
		obj.gameObject.SetActive(false);
	}

	private void OnDestroyObject(Bullet obj)
	{
		Destroy(obj.gameObject);
	}
}