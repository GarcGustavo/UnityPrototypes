using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
	public class EnemyController : MonoBehaviour
	{
		public enum MovementPattern
		{
			Sine,
			Linear,
			Curved,
			Random
		}

		[SerializeField] private float enemyId;
		[SerializeField] private float maxHealth = 100;
		[SerializeField] private float speed = 2f;
		[SerializeField] private float pauseDuration = .5f;
		[SerializeField] private MovementPattern state = MovementPattern.Linear;
		[SerializeField] private Transform[] waypointList;
		[SerializeField] private ParticleSystem deathParticles;

		private Transform _activeWaypoint;
		private float _currentHealth;
		private bool _isMoving = true;
		private Queue<Transform> _waypointQueue;

		private void Awake()
		{
			gameObject.tag = "Enemy";
			//Subscribing to global events
			CustomEventSystem.EnemyDamage += Damage;
			//CustomEventSystem.EnemyDeath += Death;
		}

		private void Start()
		{
			//Gonna switch to use enemy list/pool size as id
			enemyId = 2;
			_currentHealth = maxHealth;
			_activeWaypoint = waypointList.First();
			_waypointQueue = new Queue<Transform>();
			_waypointQueue.Enqueue(_activeWaypoint);
			foreach (var point in waypointList) _waypointQueue.Enqueue(point);
		}

		//Update is used for things that need to happen each frame, so mostly visual changes will go in here
		public virtual void Update()
		{
			if (_isMoving)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, _activeWaypoint.localPosition,
					speed * Time.deltaTime);
				//if (state == MovementPattern.Sine) transform.localPosition *= Mathf.Cos(180);
				if (transform.localPosition == _activeWaypoint.localPosition) StartCoroutine(MoveToWaypoint());
			}
		}

		private void OnDisable()
		{
			//Unsubscribing from global events
			CustomEventSystem.EnemyDamage -= Damage;
			//CustomEventSystem.EnemyDeath -= Death;
		}

		private IEnumerator MoveToWaypoint()
		{
			_isMoving = false;
			yield return new WaitForSeconds(pauseDuration);
			_isMoving = true;
			_waypointQueue.Enqueue(_activeWaypoint);
			_activeWaypoint = _waypointQueue.Dequeue();
		}

		private void Damage(GameObject obj, float dmg)
		{
			_currentHealth -= dmg;
			if (_currentHealth <= 0) Death();
		}

		private void Death()
		{
			deathParticles.transform.localPosition = transform.localPosition;
			deathParticles.Play();
			CustomEventSystem.InvokeEnemyDeath(this);
			gameObject.SetActive(false);
		}

		private void MoveHorizontal()
		{
			//float cosineValue = Mathf.Cos(radius * Mathf.PI * frequency * elapsedTime);
			// transform.DOLocalMoveX(cosineValue, Time.deltaTime);
		}

		private void MoveVertical()
		{
			//float cosineValue = Mathf.Cos(radius * Mathf.PI * frequency * elapsedTime);
			//transform.DOLocalMoveY(cosineValue, Time.deltaTime);
		}

		private void MoveForward()
		{
		}
	}
}