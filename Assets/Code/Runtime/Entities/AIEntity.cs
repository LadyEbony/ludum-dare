using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class AIEntity : CharacterEntity {

  public Transform target;

  public static AIEntity CreateEntity() {
		var obj = Instantiate(GameInitializer.Instance.aiPrefab);
		var entity = obj.GetComponent<AIEntity>();

		return entity;
	}

  public override void StartEntity() {
    base.StartEntity();

    target = GameObject.Find("target").transform;
  }

  public override void UpdateEntity() {
    if (isMine){
      agent.destination = target.position;

      var agentv = agent.velocity;
      input = new Vector2(agentv.x, agentv.z).normalized;
    }

    base.UpdateEntity();
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);
  }

}
