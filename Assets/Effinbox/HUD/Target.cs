using UnityEngine;
using UnityEngine.UI;
using Axis = UnityEngine.RectTransform.Axis;

namespace Effinbox.HUD {

  public class Target: MonoBehaviour {

    public Entity target;
    public Entity player;
    public Material normalMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;
    public Vector2 position {
      get {
        return rectTransform.localPosition;
      }
      set {
        rectTransform.localPosition = value;
      }
    }

    private RectTransform rectTransform;
    private Text nameText;
    private Text distanceText;
    private Text descriptionText;
    private Line rectSelectable;
    private Line rectSelected;
    private Line rectLocked;

    private void Start() {
      rectTransform = GetComponent<RectTransform>();
      nameText = transform.Find("Name").GetComponent<Text>();
      distanceText = transform.Find("Distance").GetComponent<Text>();
      descriptionText = transform.Find("Description").GetComponent<Text>();
      rectSelectable = transform.Find("RectSelectable").GetComponent<Line>();
      rectSelected = transform.Find("RectSelected").GetComponent<Line>();
      rectLocked = transform.Find("RectLocked").GetComponent<Line>();
    }

    public void LateUpdate() {
      if (!target || !player) {
        return;
      }
      var name = target.hudAbbr.Length > 0
        ? target.hudAbbr
        : target.name;
      var distance = Vector3.Distance(player.transform.position,
        target.transform.position);
      var size = 120 - Mathf.Clamp(Mathf.Log(2 + distance / 20) * 20, 0, 100);
      nameText.text = name;
      distanceText.text = distance.ToString("0000");
      rectTransform.sizeDelta = new Vector2(size, size);
      // Description text
      if (target.faction == player.faction) {
        SetDescriptionText("FRND", blueMaterial);
      }
      else if (target.priority == EntityPriority.Primary) {
        SetDescriptionText("TGT", redMaterial);
      }
      else if (target.priority == EntityPriority.Secondary) {
        SetDescriptionText("S.TGT", normalMaterial);
      }
      else if (target.priority == EntityPriority.Neutral) {
        SetDescriptionText("NTL", yellowMaterial);
      }
      else {
        SetDescriptionText();
      }
      // Lock status
      var radar = player.GetComponent<Radar>();
      if (radar && target.isRadarLockable && target.faction != player.faction) {
        if (radar.IsSelected(target)) {
          rectSelectable.gameObject.SetActive(false);
          rectSelected.gameObject.SetActive(true);
        } else {
          rectSelectable.gameObject.SetActive(true);
          rectSelected.gameObject.SetActive(false);
        }
        if (radar.IsLocked(target)) {
          rectLocked.gameObject.SetActive(true);
          rectLocked.material = redMaterial;
          rectSelected.material = redMaterial;
        } else {
          rectLocked.gameObject.SetActive(false);
          rectLocked.material = normalMaterial;
          rectSelected.material = normalMaterial;
        }
      }
    }

    private void SetDescriptionText(string text, Material mat) {
      descriptionText.gameObject.SetActive(true);
      descriptionText.text = text;
      descriptionText.material = mat;
    }

    private void SetDescriptionText() {
      descriptionText.gameObject.SetActive(false);
    }

  }

}
