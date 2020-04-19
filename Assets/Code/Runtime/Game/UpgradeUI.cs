using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour {

  public static List<UpgradeUI> upgrades;

  static UpgradeUI(){
    upgrades = new List<UpgradeUI>();
  }

  public Upgrade upgrade;  
  public Canvas canvas;
  public TextMeshProUGUI title;
  public TextMeshProUGUI description;
  public Button button;

  private void Start() {
    upgrades.Add(this);
  }

  private void OnDestroy() {
    upgrades.Remove(this);
  }

  public void SetUpgrade(Upgrade u){
    upgrade = u;
    title.text = u.title;
    description.text = u.description;

    button.onClick.RemoveAllListeners();
    button.onClick.AddListener(
      () => {
        u.OnActivate();
        ClearUpgrades();
      }
    );

    canvas.enabled = true;
  }

  public void ClearUpgrades(){
    foreach(var u in upgrades){
      u.canvas.enabled = false;
    }
  }
}
