using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.AI;

public class PlayerEntity : CharacterEntity {

	public static Dictionary<int, PlayerEntity> players;

	static PlayerEntity() {
		players = new Dictionary<int, PlayerEntity>();
	}

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
	
	public static PlayerEntity CreateEntity() {
		GameObject obj = Instantiate(GameInitializer.Instance.playerPrefab);
		PlayerEntity entity = obj.GetComponent<PlayerEntity>();

		return entity;
	}

	public override void StartEntity(){
		base.StartEntity();

		UnitManager.Local.players.Add(authorityID, this);
	}

	public override void UpdateEntity() {
    if (isMine){
      // movement
			float hor = Input.GetAxisRaw("Horizontal");
			float ver = Input.GetAxisRaw("Vertical");
      input = new Vector2(hor, ver).normalized;

      MovementUpdate(Time.deltaTime);
    }

		base.UpdateEntity();

		if (isMine) {
			HandleGun();
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

	public override void DestroyEntity() {
		base.DestroyEntity();

		UnitManager.Local.players.Remove(authorityID);
	}

}
