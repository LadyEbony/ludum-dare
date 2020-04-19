using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSimpleStat : Upgrade {

  public float ammoModifier = 1f;
  public float reloadModifier = 1f;
  public float firerateModifier = 1f;
  public float projectileModifer = 1f;

  public override void OnActivate() {
    var player = UnitManager.LocalPlayer;
    if (player){
      player.ammoMax = Mathf.RoundToInt(player.ammoMax * ammoModifier);
      player.reload = player.reload * (1f / reloadModifier);
      player.fireRate = player.fireRate * firerateModifier;
      player.projectileSpeed = player.projectileSpeed * projectileModifer;
    }
  }
}
