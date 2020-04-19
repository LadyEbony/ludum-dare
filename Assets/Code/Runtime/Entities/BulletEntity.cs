using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class BulletEntity : EntityUnit
{
	
	public static BulletEntity CreateEntity()
	{
		var obj = Instantiate(GameInitializer.Instance.bulletPrefab, WorldParent.instance.transform);
		var entity = obj.GetComponent<BulletEntity>();
		return entity;
	}

	public Vector3 destination;
	public float speed;

	// TODO: THIS IS LOCAL STUFF. PLEASE NETWORK!!!!
	public DamageArgs damageArgs;
	// END TODO

	public override void StartEntity()
	{
		base.StartEntity();

		PlayerEntity player;
		if (!UnitManager.Local.players.TryGetValue(authorityID, out player))
		{
			return;
		}

		transform.position = Vector3.MoveTowards(player.transform.position, destination, player.agent.radius + 0.1f);

	}

	public override void UpdateEntity()
	{
		base.UpdateEntity();

		transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

		if (isMine)
		{
			if (Vector3.SqrMagnitude(transform.position - destination) < 0.01f)
			{
				UnitManager.Local.Deregister(this);
				Destroy(gameObject);
			}
		}
	}

	public override void Serialize(ExitGames.Client.Photon.Hashtable h)
	{
		base.Serialize(h);

		h.Add('d', destination);
	}

	public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
	{
		base.Deserialize(h);

		object val;
		if (h.TryGetValue('d', out val))
		{
			destination = (Vector3)val;
		}
	}

	// TODO: THIS IS LOCAL STUFF. PLEASE NETWORK!!!!
	private void OnCollisionEnter(Collision collision)
	{
		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if(damageable != null)
		{
			damageable.TakeDamage(damageArgs);
		}
		UnitManager.Local.Deregister(this);
		Destroy(gameObject);
	}
	// END TODO
}
