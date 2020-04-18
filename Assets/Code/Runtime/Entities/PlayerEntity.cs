using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerEntity : EntityUnit {

  public static Dictionary<int, PlayerEntity> players;

  static PlayerEntity(){
    players = new Dictionary<int, PlayerEntity>();
  }

  [Header("Gameplay")]
  public float speed = 5f;

  [Header("Network")]
  public float prevNetworkTime;

  public Vector3 prevPos;
  public Vector3 nextPos;
  public Vector3 dampVector;

  public static PlayerEntity CreateEntity(){
    var obj = Instantiate(GameInitializer.Instance.playerPrefab);
    var entity = obj.GetComponent<PlayerEntity>();
    return entity;
  }

  public override void StartEntity() {
    base.StartEntity();

    UnitManager.Local.players.Add(authorityID, this);
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    if (isMine){
      // movement
      var hor = Input.GetAxisRaw("Horizontal");
      var ver = Input.GetAxisRaw("Vertical");

      transform.position += new Vector3(hor, 0f, ver) * Time.deltaTime * speed;

      // gun
      if (Input.GetMouseButton(0)){
        var bullet = BulletEntity.CreateEntity();
        
        var playersp = Camera.main.WorldToScreenPoint(transform.position);
        var camerasp = Input.mousePosition;
        var dir = (camerasp - playersp).normalized;

        bullet.destination = dir * 10f;

        UnitManager.Local.Register(bullet);

      }
    } else {
      // intrepolation AND extrapolation
      var lerpNetwork = Time.time - prevNetworkTime;
      var extraPos = Vector3.Lerp(prevPos, nextPos, lerpNetwork / updateTimer);
      
      transform.position = Vector3.SmoothDamp(transform.position, extraPos, ref dampVector, Time.deltaTime * 2f);
    }
  }

  public override void DestroyEntity() {
    base.DestroyEntity();

    UnitManager.Local.players.Remove(authorityID);
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('p', transform.position);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);
    
    prevNetworkTime = Time.time;

    // p
    object val;
    if (h.TryGetValue('p', out val)){
      prevPos = transform.position;
      nextPos = (Vector3)val;
    }
    
  }

}
