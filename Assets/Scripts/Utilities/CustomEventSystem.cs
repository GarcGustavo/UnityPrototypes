using System;
using Enemy;
using MoreMountains.Feedbacks;
using UnityEngine;

public class CustomEventSystem : MonoBehaviour
{
	//Global list of events, int is used to track object id's
	//Call _instance from any other class to invoke or subscribe to global events

	//Player
	public static event Action<int> PlayerHeal;
	public static event Action<int> PlayerDamage;
	public static event Action PlayerDeath;

	//Camera
	public static event Action<MMFeedbacks> CameraShake;
	public static event Action<MMFeedbacks> CameraZoom;
	public static event Action<MMFeedbacks> CameraFOV;
	public static event Action<MMFeedbacks> CameraDistortion;
	public static event Action TopdownCamera;

	//Enemy
	public static event Action<EnemyController> EnemyShoot;
	public static event Action<EnemyController> EnemyDeath;
	public static event Action<EnemyController> EnemyHeal;
	public static event Action<GameObject, float> EnemyDamage;

	//Bullet
	public static event Action<Bullet> BulletHit;
	public static event Action<Bullet> BulletDisable;
	public static event Action<Bullet> BulletEnable;

	//System
	public static event Action LevelStart;
	public static event Action LevelEnd;

	//UI
	public static event Action<int> Pause;
	public static event Action<int> MainMenu;
	public static event Action<int> OptionsMenu;
	public static event Action<int> HideMenu;
	public static event Action<int> OpenMenu;

	//Global Invoke Methods

	//Player
	public static void InvokePlayerDeath()
	{
		PlayerDeath?.Invoke();
	}

	public static void InvokePlayerHeal(int val)
	{
		PlayerHeal?.Invoke(val);
	}

	public static void InvokePlayerDamage(int val)
	{
		PlayerDamage?.Invoke(val);
	}

	//Camera
	public static void InvokeCameraShake(MMFeedbacks obj)
	{
		CameraShake?.Invoke(obj);
	}

	public static void InvokeCameraZoom(MMFeedbacks obj)
	{
		CameraZoom?.Invoke(obj);
	}

	public static void InvokeCameraFOV(MMFeedbacks obj)
	{
		CameraFOV?.Invoke(obj);
	}

	public static void InvokeCameraDistortion(MMFeedbacks obj)
	{
		CameraDistortion?.Invoke(obj);
	}

	public static void InvokeTopdownCamera()
	{
		TopdownCamera?.Invoke();
	}

	//Enemy
	public static void InvokeEnemyShoot(EnemyController obj)
	{
		EnemyShoot?.Invoke(obj);
	}

	public static void InvokeEnemyDeath(EnemyController obj)
	{
		EnemyDeath?.Invoke(obj);
	}

	public static void InvokeEnemyHeal(EnemyController obj)
	{
		EnemyHeal?.Invoke(obj);
	}

	public static void InvokeEnemyDamage(GameObject obj, float dmg)
	{
		EnemyDamage?.Invoke(obj, dmg);
	}

	//Bullet
	public static void InvokeBulletHit(Bullet obj)
	{
		BulletHit?.Invoke(obj);
	}

	public static void InvokeBulletDisable(Bullet obj)
	{
		BulletDisable?.Invoke(obj);
	}

	public static void InvokeBulletEnable(Bullet obj)
	{
		BulletEnable?.Invoke(obj);
	}

	//System
	public static void InvokeLevelStart()
	{
		LevelStart?.Invoke();
	}

	public static void InvokeLevelEnd()
	{
		LevelEnd?.Invoke();
	}

	//UI
	public static void InvokePause(int obj)
	{
		Pause?.Invoke(obj);
	}

	public static void InvokeMainMenu(int obj)
	{
		MainMenu?.Invoke(obj);
	}

	public static void InvokeOptionsMenu(int obj)
	{
		OptionsMenu?.Invoke(obj);
	}

	public static void InvokeHideMenu(int obj)
	{
		HideMenu?.Invoke(obj);
	}

	public static void InvokeOpenMenu(int obj)
	{
		OpenMenu?.Invoke(obj);
	}
}