using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.AI;

public abstract class CharacterEntity : EntityUnit {

	[Header("Gameplay")]
  public int health = 10;
  public float movementSpeed = 10f;
  [ReadOnly] public NavMeshAgent agent;

	[Header("Network")]
  public float desyncTimer = 0f;
  public Vector2 input;
  public Vector3 netPosition;

  public override void AwakeEntity() {
    base.AwakeEntity();

    agent = GetComponent<NavMeshAgent>();
  }

  public override void StartEntity() {
		base.StartEntity();

		if(!isMine){
      agent.Warp(netPosition);
		}
	}

	public override void UpdateEntity(){
		base.UpdateEntity();

		if (!isMine) {
      // fix position if we off
      var dist = Vector3.Distance(transform.position, netPosition);

      var deadReckonTime = NetworkManager.net.loadBalancingPeer.RoundTripTime * 0.001f;
      deadReckonTime = Mathf.Clamp(deadReckonTime, updateTimer, 1f);
      var speed = agent.velocity.magnitude * 2f;

      // aggressively get into position every frame
      transform.position = Vector3.Lerp(transform.position, netPosition, Time.deltaTime * (input.sqrMagnitude < 0.1f ? 16f : 2f));

      // moving far from reckon zone
      if (dist > (deadReckonTime * speed)){
        desyncTimer += Time.deltaTime;
      } 
      // moving within reckon move
      else {
        desyncTimer = Mathf.Max(desyncTimer - Time.deltaTime, 0f);
      }

      // put a timer, only aggressively fix it when it's been too long
      if (desyncTimer >= 0.5f ){
        transform.position = netPosition;
      }

      // extrapolation? use inputs
      MovementUpdate(Time.deltaTime);
    }
	}

  public virtual void MovementUpdate(float delta){
    agent.velocity = Vector3.MoveTowards(agent.velocity, new Vector3(input.x, 0f, input.y) * agent.speed, agent.acceleration * delta);
  }

	public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
		base.Serialize(h);

		h.Add('p', transform.position);
    h.Add('i', input);
    h.Add('v', agent.velocity);

    h.Add('h', health);
	}

	public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
		base.Deserialize(h);

    object val;

		if (h.TryGetValue('p', out val)) {
			netPosition = (Vector3)val;
		}
    if (h.TryGetValue('i', out val)){
      input = (Vector2)val;
    }
    if (h.TryGetValue('v', out val)){
      agent.velocity = (Vector3)val;
    }

    // h
    if (h.TryGetValue('h', out val)){
      health = (int)val;
    }

	}

}
