using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.AI;

public class PlayerEntity : EntityUnit
{
	public static Dictionary<int, PlayerEntity> players;

	static PlayerEntity()
	{
		players = new Dictionary<int, PlayerEntity>();
	}

	[Header("Gameplay Movement")]
  public float movementSpeed = 10f;
  private NavMeshAgent agent;

  [Header("Gameplay Gun")]
  public int ammo = 30;
  public int ammoMax = 30;
  public float fireRate = 60f;
  public float reload = 1f;
  public float projectileSpeed = 10f;
  public float projectileDeviation = 2f;

  public event System.Action<PlayerEntity> onBullet;

  private float nextAmmoReady;
  private float nextReadyReady;
	
	[Header("Network")]
	public float prevNetworkTime;

	public Vector3 prevPos;
	public Vector3 nextPos;
	public Vector3 dampVector;

	public static PlayerEntity CreateEntity()
	{
		GameObject obj = Instantiate(GameInitializer.Instance.playerPrefab);
		PlayerEntity entity = obj.GetComponent<PlayerEntity>();

		return entity;
	}

	public override void StartEntity()
	{
		base.StartEntity();

		UnitManager.Local.players.Add(authorityID, this);

		agent = GetComponent<NavMeshAgent>();

		if(!isMine)
		{
			transform.position = nextPos;
		}
	}

	public override void UpdateEntity()
	{
		base.UpdateEntity();

		if (isMine)
		{
			// movement
			float hor = Input.GetAxisRaw("Horizontal");
			float ver = Input.GetAxisRaw("Vertical");
			Vector3 direction = Vector3.ClampMagnitude(new Vector3(hor, 0, ver), 1);
			Vector3 velocity = direction * movementSpeed;
			agent.Move(velocity * Time.deltaTime);

			HandleGun();
		}
		else
		{
			// intrepolation AND extrapolation
			float lerpNetwork = Time.time - prevNetworkTime;
			Vector3 extraPos = Vector3.Lerp(prevPos, nextPos, lerpNetwork / updateTimer);

			agent.SetDestination(Vector3.SmoothDamp(transform.position, extraPos, ref dampVector, Time.deltaTime * 2f));
		}
	}

  private void HandleGun(){
    if (Input.GetMouseButton(0) && ammo > 0 && Time.time >= nextAmmoReady){
      CreateBullet();

      onBullet?.Invoke(this);

      ammo -= 1;
      nextAmmoReady = Time.time + 1 / fireRate;

      if (ammo == 0){
        nextReadyReady = Time.time + reload;
      }
    }

    if (ammo == 0 && Time.time > nextReadyReady){
      ammo = ammoMax;
    }
  }

  public void CreateBullet(){
    BulletEntity bullet = BulletEntity.CreateEntity();

    var playerPosition = transform.position;
    var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    var tRay = (playerPosition.y - cameraRay.origin.y) / cameraRay.direction.y;
    var cameraPosition = cameraRay.origin + tRay * cameraRay.direction;

    var dir = Quaternion.AngleAxis(Random.Range(-projectileDeviation, projectileDeviation), Vector3.up) * (cameraPosition - playerPosition).normalized;

		UnitManager.Local.Register(bullet);

    bullet.destination = bullet.transform.position + dir * 10f;
    bullet.speed = projectileSpeed;
  }

	public override void DestroyEntity()
	{
		base.DestroyEntity();

		UnitManager.Local.players.Remove(authorityID);
	}

	public override void Serialize(ExitGames.Client.Photon.Hashtable h)
	{
		base.Serialize(h);

		h.Add('p', transform.position);
	}

	public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
	{
		base.Deserialize(h);

		prevNetworkTime = Time.time;

		// p
		object val;
		if (h.TryGetValue('p', out val))
		{
			prevPos = transform.position;
			nextPos = (Vector3)val;
		}

	}

}
