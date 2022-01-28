using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private float speed = 50f;
	[SerializeField] private float timeLimit = 1f;
	[SerializeField] private float timeActive;
	[SerializeField] private float dmg = 1;
	[SerializeField] private bool inPool = true;
	[SerializeField] private Vector3 direction = Vector3.forward;

	[SerializeField] private ParticleSystem ImpactSystem;

	[SerializeField] private TrailRenderer trail;
	//private Rigidbody rb;

	// Start is called before the first frame update
	private void Awake()
	{
		//rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	private void Update()
	{
		transform.localPosition += direction * speed * Time.deltaTime;
		timeActive = inPool ? timeActive + Time.deltaTime : 0f;

		if (timeActive >= timeLimit) Disable?.Invoke(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		//rb.velocity = Vector3.zero;

		if (other.gameObject.CompareTag("Enemy"))
		{
			//ImpactSystem.transform.forward = -1 * transform.forward;
			//ImpactSystem.Play();
			//Damage?.Invoke(this, dmg);
			CustomEventSystem.InvokeEnemyDamage(other.gameObject, dmg);
			Disable?.Invoke(this);
			inPool = true;
		}
	}

	public static event Action<Bullet> Disable;
	public static event Action<Bullet, float> Damage;

	public void SetDirection(Vector3 dir, float vel)
	{
		speed = vel;
		direction = dir;
		inPool = false;
		transform.rotation = Quaternion.identity;
	}

	public void ClearTrail()
	{
		//trail.Play(state);
		//trail.Clear();
	}
}