using System.Collections;
using Cinemachine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(CinemachineDollyCart))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private bool topDown;
	[SerializeField] private float cameraTransitionDuration = 0.5f;

	//MMFeedback components to add controller oomf
	[SerializeField] private Transform playerModel;
	[SerializeField] private MMFeedbacks shakeFeedbacks;
	[SerializeField] private MMFeedbacks boostFeedbacks;
	[SerializeField] private MMFeedbacks brakeFeedbacks;

	//Aiming related components
	[SerializeField] private LayerMask aimLayer;
	[SerializeField] private Transform aimTarget;
	[SerializeField] private Transform cameraParent;
	[SerializeField] private CinemachineVirtualCamera DefaultCamera;
	[SerializeField] private CinemachineVirtualCamera BackVirtualCamera;
	[SerializeField] private CinemachineVirtualCamera TopVirtualCamera;

	//TODO: Move to player scriptable object
	[SerializeField] private float maxHP;
	[SerializeField] private float currentHP;
	[SerializeField] private float playerSpeed;
	[SerializeField] private float boostModifier;
	[SerializeField] private float brakeModifier;
	[SerializeField] private float rotationSpeed;
	[SerializeField] private float leanLimit = 80f;
	[SerializeField] private float leanLerpTime = 0.1f;
	[SerializeField] private Vector2 movementLimit = new Vector2(5, 3);
	private Transform cart;

	private CinemachineVirtualCamera CurrentCamera;

	//Private variables not exposed to editor
	private CinemachineDollyCart dolly;
	private CinemachineImpulseSource impulseCamSource;
	private bool playedBoost;
	private bool playedBrake;
	private bool switchingView;

	//TODO:
	// Add space/jump input to replace roll as dodge that pushes you in current movement direction
	// Fix screen clamp jumping when switching perspectives

	private void Awake()
	{
		//Subscribing to input events
		InputController.QuickSpin += QuickSpin;
		InputController.Boost += Boost;
		InputController.Brake += Brake;
		InputController.Move += MovePlayer;
		InputController.Look += RotationLook;
		//Subscribing to global events
		CustomEventSystem.PlayerHeal += Heal;
		CustomEventSystem.PlayerDamage += Damage;
		CustomEventSystem.PlayerDeath += Die;
		CustomEventSystem.TopdownCamera += ToggleTopdownView;
	}

	private void Start()
	{
		cart = transform.parent.transform;
		aimTarget.parent.position = transform.position;
		dolly = transform.parent.GetComponent<CinemachineDollyCart>();
		CurrentCamera = DefaultCamera;
		CurrentCamera.Priority += 1;
		//playerVirtualCamera = cameraParent.GetComponentInChildren<CinemachineVirtualCamera>();


		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;

		SetSpeed(playerSpeed);
	}

	private void OnDisable()
	{
		//Unsubscribing from input events
		InputController.QuickSpin -= QuickSpin;
		InputController.Boost -= Boost;
		InputController.Brake -= Brake;
		InputController.Move -= MovePlayer;
		InputController.Look -= RotationLook;
		//Unsubscribing from global events
		CustomEventSystem.PlayerHeal -= Heal;
		CustomEventSystem.PlayerDamage -= Damage;
		CustomEventSystem.PlayerDeath -= Die;
		CustomEventSystem.TopdownCamera -= ToggleTopdownView;
	}

	private void OnDrawGizmos()
	{
		//Aim Object
		//Gizmos.color = Color.blue;
		//Gizmos.DrawWireSphere(aimTarget.position, .5f);
		//Gizmos.DrawSphere(aimTarget.position, .15f);

		//Camera bounds
		var tmp = transform.parent.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(
			new Vector3(tmp.localPosition.x - movementLimit.x, tmp.localPosition.y - movementLimit.y,
				tmp.localPosition.z),
			new Vector3(tmp.localPosition.x + movementLimit.x, tmp.localPosition.y - movementLimit.y,
				tmp.localPosition.z));

		Gizmos.DrawLine(
			new Vector3(tmp.localPosition.x - movementLimit.x, tmp.localPosition.y + movementLimit.y,
				tmp.localPosition.z),
			new Vector3(tmp.localPosition.x + movementLimit.x, tmp.position.y + movementLimit.y, tmp.localPosition.z));

		Gizmos.DrawLine(
			new Vector3(tmp.localPosition.x - movementLimit.x, tmp.localPosition.y - movementLimit.y, tmp.position.z),
			new Vector3(tmp.localPosition.x - movementLimit.x, tmp.position.y + movementLimit.y, tmp.localPosition.z));

		Gizmos.DrawLine(
			new Vector3(tmp.localPosition.x + movementLimit.x, tmp.localPosition.y - movementLimit.y,
				tmp.localPosition.z),
			new Vector3(tmp.localPosition.x + movementLimit.x, tmp.localPosition.y + movementLimit.y,
				tmp.localPosition.z));
	}

	private void Die()
	{
		gameObject.SetActive(false);
	}

	private void Heal(int val)
	{
		currentHP += val;
		if (currentHP >= maxHP) currentHP = maxHP;
	}

	private void Damage(int val)
	{
		currentHP -= val;
		if (currentHP <= 0) CustomEventSystem.InvokePlayerDeath();
	}

	public void ToggleTopdownView()
	{
		switchingView = true;
		//BROKEN REMEMBER TO FIX
		if (!topDown)
		{
			topDown = true;
			TopVirtualCamera.Priority = CurrentCamera.Priority + 1;
			CurrentCamera = TopVirtualCamera;
		}
		else
		{
			topDown = false;
			BackVirtualCamera.Priority = CurrentCamera.Priority + 1;
			CurrentCamera = BackVirtualCamera;
		}

		StartCoroutine(LerpView());
		//Vector3.Lerp(transform.position, ClampPosition(), .1f);
	}

	private void SetSpeed(float x)
	{
		dolly.m_Speed = x;
	}

	private void MovePlayer(Vector3 input)
	{
		if (!switchingView)
		{
			if (topDown)
			{
				input.z = input.y;
				input.y = 0;
			}

			transform.localPosition += input * playerSpeed * Time.deltaTime;
			transform.position = ClampPosition();
		}
	}

	//TODO: fix or change to Movetowards or DOTween to move at constant rate, not sure if timeElapsed is correct method
	private IEnumerator LerpView()
	{
		switchingView = true;
		var timeElapsed = 0f;
		//while (timeElapsed < cameraTransitionDuration)
		//{
		//transform.position = Vector3.Lerp(transform.position, ClampPosition(), timeElapsed/cameraTransitionDuration);
		//timeElapsed += Time.deltaTime;
		//yield return null;
		//}
		//transform.position = ClampPosition();
		transform.DOMove(ClampPosition(), cameraTransitionDuration);
		yield return new WaitForSeconds(cameraTransitionDuration);
		switchingView = false;
	}

	private Vector3 ClampPosition()
	{
		//Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		if (topDown)
		{
			var viewPos = Camera.main.WorldToViewportPoint(transform.position);
			viewPos.x = Mathf.Clamp01(viewPos.x);
			viewPos.y = Mathf.Clamp01(viewPos.y);
			return new Vector3(Camera.main.ViewportToWorldPoint(viewPos).x, cart.position.y,
				Camera.main.ViewportToWorldPoint(viewPos).z);

			//float posX = Mathf.Clamp(transform.position.x, cart.position.x - movementLimit.x, cart.position.x + movementLimit.x);
			//float posZ = Mathf.Clamp(transform.position.z, cart.position.z - movementLimit.y, cart.position.z + movementLimit.y);
			//transform.position = new Vector3(posX, cart.position.y, posZ );
		}

		var posX = Mathf.Clamp(transform.position.x, cart.position.x - movementLimit.x,
			cart.position.x + movementLimit.x);
		var posY = Mathf.Clamp(transform.position.y, cart.position.y - movementLimit.y,
			cart.position.y + movementLimit.y);
		return new Vector3(posX, posY, cart.position.z);
	}

	//TODO: switch targeting to use aimTarget movement + Tween DOLookat
	private void RotationLook(Vector3 input)
	{
		// Update player rotation to aimobject/reticle
		aimTarget.parent.localPosition = Vector3.zero;

		if (!switchingView)
		{
			// Horizontal leaning based on input direction
			var targetEulerAngels = playerModel.localEulerAngles;
			if (topDown)
				playerModel.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y,
					Mathf.LerpAngle(targetEulerAngels.z, -input.x * leanLimit, leanLerpTime));
			else
				playerModel.localEulerAngles = new Vector3(
					Mathf.LerpAngle(targetEulerAngels.x, -input.y * leanLimit / 2, leanLerpTime),
					Mathf.LerpAngle(targetEulerAngels.y, input.x * leanLimit / 2, leanLerpTime),
					Mathf.LerpAngle(targetEulerAngels.z, -input.x * leanLimit, leanLerpTime));
		}
	}

	private void Boost(bool state)
	{
		if (state)
		{
			if (!playedBoost && !playedBrake)
			{
				//Change camera values to simulate speed
				boostFeedbacks?.PlayFeedbacks();
				playedBoost = true;
			}

			shakeFeedbacks?.PlayFeedbacks();
			playerSpeed += boostModifier;
		}
		else
		{
			if (playedBoost && !playedBrake)
			{
				//Reverse boost camera values
				boostFeedbacks?.StopFeedbacks();
				boostFeedbacks?.PlayFeedbacks();
				//boostFeedbacks?.Revert();
				playedBoost = false;
			}

			shakeFeedbacks?.StopFeedbacks();
			playerSpeed -= boostModifier;
		}
	}


	private void Brake(bool state)
	{
		if (state)
		{
			if (!playedBrake && !playedBoost)
			{
				brakeFeedbacks?.PlayFeedbacks();
				playedBrake = true;
			}

			shakeFeedbacks?.PlayFeedbacks();
			playerSpeed -= brakeModifier;
		}
		else
		{
			if (playedBrake && !playedBoost)
			{
				//Reverse brake camera values
				brakeFeedbacks?.StopFeedbacks();
				brakeFeedbacks?.PlayFeedbacks();
				playedBrake = false;
			}

			shakeFeedbacks?.StopFeedbacks();
			playerSpeed += brakeModifier;
		}
	}

	private void QuickSpin(int triggerDir, Vector3 direction)
	{
		//transform.DOLocalRotate(transform.position, .5f);

		if (!DOTween.IsTweening(playerModel))
			playerModel.DOLocalRotate(
				new Vector3(playerModel.localEulerAngles.x, playerModel.localEulerAngles.y, 360 * -triggerDir), .5f,
				RotateMode.FastBeyond360).SetEase(Ease.OutSine);
		//playerModel.DOLocalMove(direction * 2 , 0.1f);
	}
}