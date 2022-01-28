using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Hazard") OnDamage?.Invoke(1);
	}

	public static event Action<int> OnDamage;
}