using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
	private Vector3 input;

	private bool playerCanMove;

	private void Awake()
	{
		playerCanMove = true;
	}

	// Update is called once per frame
	private void Update()
	{
		PlayerInput();
	}

	public static event Action<bool, Vector3> Shoot;
	public static event Action<bool> Boost;
	public static event Action<bool> Brake;
	public static event Action<int, Vector3> QuickSpin;
	public static event Action<Vector3> Move;
	public static event Action<Vector3> Look;

	//TODO: Rename input key bindings to be more intuitive
	private void PlayerInput()
	{
		if (playerCanMove)
		{
			input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
			Move?.Invoke(input);
			Look?.Invoke(input);

			if (Input.GetButtonDown("Fire3") && !Input.GetButton("Fire1"))
				Boost?.Invoke(true);
			if (Input.GetButtonUp("Fire3") && !Input.GetButton("Fire1"))
				Boost?.Invoke(false);

			if (Input.GetButtonDown("Fire1") && !Input.GetButton("Fire3"))
				Brake?.Invoke(true);
			if (Input.GetButtonUp("Fire1") && !Input.GetButton("Fire3"))
				Brake?.Invoke(false);

			if (Input.GetButton("Shoot1"))
				Shoot?.Invoke(true, transform.forward);
			if (Input.GetButtonUp("Shoot1"))
				Shoot?.Invoke(false, transform.forward);

			if (Input.GetButtonDown("Debug"))
				CustomEventSystem.InvokeTopdownCamera();

			if (Input.GetButtonDown("TriggerL") || Input.GetButtonDown("TriggerR"))
			{
				var dir = Input.GetButtonDown("TriggerL") ? -1 : 1;
				QuickSpin?.Invoke(dir, input);
			}
		}
	}
}