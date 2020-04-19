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

	[Header("Gameplay")]
	public float speed = 5f;
	public NavMeshAgent agent;
	private Camera cam;

	[Header("Network")]
	public float prevNetworkTime;

	public Vector3 prevPos;
	public Vector3 nextPos;
	public Vector3 dampVector;

	public static PlayerEntity CreateEntity()
	{
		GameObject obj = Instantiate(GameInitializer.Instance.playerPrefab, WorldParent.instance.transform);
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
			Vector3 velocity = direction * speed;
			agent.Move(velocity * Time.deltaTime);

			// gun
			if (Input.GetMouseButton(0))
			{
				BulletEntity bullet = BulletEntity.CreateEntity();

				//Vector3 playersp = cam.WorldToScreenPoint(transform.position);
				//Vector3 camerasp = Input.mousePosition;
				//Vector3 dir = (camerasp - playersp).normalized;
				Vector3 playerXZPos = new Vector3(transform.position.x, 0, transform.position.z);
				Vector3 cursorXZPos = ScreenToXZPoint(Input.mousePosition, transform.position.y);
				Vector3 dir = (cursorXZPos - playerXZPos).normalized;

				bullet.destination = transform.position + dir * 10f;

				UnitManager.Local.Register(bullet);

			}
		}
		else
		{
			// intrepolation AND extrapolation
			float lerpNetwork = Time.time - prevNetworkTime;
			Vector3 extraPos = Vector3.Lerp(prevPos, nextPos, lerpNetwork / updateTimer);

			agent.SetDestination(Vector3.SmoothDamp(transform.position, extraPos, ref dampVector, Time.deltaTime * 2f));
		}
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

	private void Start()
	{
		cam = Camera.main;
	}

	private Vector3 ScreenToXZPoint (Vector3 screenPos, float y = 0)
	{
		// Assumes the camera is ONLY rotated around the x-axis
		Vector3 worldCamPos = cam.ScreenToWorldPoint(screenPos);
		Vector3 groundPos = new Vector3(worldCamPos.x, 0, worldCamPos.z - (worldCamPos.y - y) / Mathf.Tan(-cam.transform.eulerAngles.x * Mathf.Deg2Rad));
		return groundPos;
	}

	private void OnDrawGizmos()
	{
		Vector3 playerXZPos = new Vector3(transform.position.x, 0, transform.position.z);
		Vector3 cursorXZPos = ScreenToXZPoint(Input.mousePosition, transform.position.y);
		Debug.DrawLine(playerXZPos, cursorXZPos, Color.green);
		Vector3 dir = (cursorXZPos - playerXZPos).normalized;
		Debug.DrawRay(transform.position, dir * 10, Color.red);
	}

}
