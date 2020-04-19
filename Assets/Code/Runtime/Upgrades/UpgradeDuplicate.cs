using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDuplicate : Upgrade {

  [Header("Values")]
  public float chance;

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.onBullet += OnBullet;
    }
    player.projectileDeviation = player.projectileDeviation * 4f;
  }

  public void OnBullet(PlayerEntity player){
    if (Random.value <= chance){
      player.CreateBullet();
    }
  }

}
