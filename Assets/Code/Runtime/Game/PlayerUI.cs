using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour {

  public TextMeshProUGUI textMesh;

  // Update is called once per frame
  void Update(){
    var player = UnitManager.LocalPlayer;
    if (player){
      textMesh.text = string.Format("{0}/{1}", player.ammo, player.ammoMax); 
    }
  }
}
