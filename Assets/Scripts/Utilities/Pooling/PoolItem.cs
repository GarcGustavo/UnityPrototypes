using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PoolItem : MonoBehaviour
{
	[SerializeField] private ParticleSystem ImpactSystem;

	private Rigidbody Rigidbody;

	//public static event Action Shoot;

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}

	private void OnParticleSystemStopped()
	{
		Disable?.Invoke(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		ImpactSystem.transform.forward = -1 * transform.forward;
		ImpactSystem.Play();
		Rigidbody.velocity = Vector3.zero;
	}

	public static event Action<PoolItem> Disable;

	public void Shoot(Vector3 Position, Vector3 Direction, float Speed)
	{
		Rigidbody.velocity = Vector3.zero;
		transform.position = Position;
		transform.forward = Direction;

		Rigidbody.AddForce(Direction * Speed, ForceMode.VelocityChange);
	}
}