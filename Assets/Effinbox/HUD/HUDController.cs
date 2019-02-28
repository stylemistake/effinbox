using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Effinbox.HUD {

  public class HUDController: MonoBehaviour {

    public Entity player;
    public new Camera camera;
    public Target hudTargetPrefab;

    private Rect screen;
    private Rigidbody playerRigidbody;
    // private AircraftPhysics playerAircraft;
    private Radar playerRadar;
    private Health playerHealth;
    private readonly Dictionary<Entity, Target> hudTargetMap = new Dictionary<Entity, Target>();

    private Text speedText;
    private RectTransform speedSpinner;
    private Text pitchTextLeft;
    private Text pitchTextRight;
    private RectTransform velocityPointer;
    private Text healthText;

    public void Start() {
      screen = GetComponent<RectTransform>().rect;
      playerRigidbody = player.GetComponent<Rigidbody>();
      // playerAircraft = player.GetComponent<AircraftPhysics>();
      playerRadar = player.GetComponent<Radar>();
      playerHealth = player.GetComponent<Health>();
      speedText = transform.FindChild("SpeedText").GetComponent<Text>();
      speedSpinner = transform.FindChild("SpeedSpinner").GetComponent<RectTransform>();
      pitchTextLeft = transform.FindChild("PitchTextLeft").GetComponent<Text>();
      pitchTextRight = transform.FindChild("PitchTextRight").GetComponent<Text>();
      velocityPointer = transform.FindChild("VelocityPointer").GetComponent<RectTransform>();
      healthText = transform.FindChild("HealthText").GetComponent<Text>();
    }

    public float GetPitchDegrees() {
      var pitch = player.transform.rotation.eulerAngles.x;
      return Mathf.PingPong(pitch, 180) * (pitch < 180 ? -1 : 1);
    }

    public Vector2 GetAxisAngles(Vector3 vec) {
      if (vec.magnitude <= 0) {
        return Vector2.zero;
      }
      var up = player.transform.up;
      var forward = player.transform.forward;
      var heading = vec.normalized;
      var rotation = Quaternion.Inverse(Quaternion.LookRotation(forward, up))
        * Quaternion.LookRotation(heading, up);
      var angles = rotation.eulerAngles;
      return new Vector2(
        Mathf.PingPong(angles.y, 180) * (angles.y < 180 ? 1 : -1),
        Mathf.PingPong(angles.x, 180) * (angles.x < 180 ? -1 : 1)) / 180;
    }

    public Vector2? GetProjection(Vector3 position) {
      var canvas = GetComponent<Canvas>();
      var viewportPoint = camera.WorldToScreenPoint(position);
      if (viewportPoint.z <= 0) {
        return null;
      }
      viewportPoint.x -= camera.pixelWidth / 2;
      viewportPoint.y -= camera.pixelHeight / 2;
      var point = new Vector2(viewportPoint.x, viewportPoint.y)
        / canvas.scaleFactor;
      return point;
    }

    public void LateUpdate() {
      var speed = playerRigidbody.velocity.magnitude;
      if (Application.isPlaying) {
        speedText.text = Util.Kph(speed).ToString("0");
      }
      speedSpinner.localRotation = Quaternion.Euler(0f, 0f, -speed * 45);

      var health = playerHealth.health;
      healthText.text = health.ToString("0");

      var pitchDegrees = GetPitchDegrees();
      pitchTextRight.text = pitchTextLeft.text = pitchDegrees.ToString("00");

      var velocityAngles = GetAxisAngles(playerRigidbody.velocity)
        * screen.width * 2;
      velocityPointer.localPosition = velocityAngles;

      foreach (var entity in playerRadar.GetTargets()) {
        Target hudTarget;
        if (!hudTargetMap.ContainsKey(entity)) {
          hudTarget = (Target)Instantiate(hudTargetPrefab, transform.Find("Targets"));
          hudTarget.transform.localScale = Vector3.one;
          hudTarget.target = entity;
          hudTarget.player = player;
          hudTargetMap.Add(entity, hudTarget);
        } else {
          var hudPosition = GetProjection(entity.transform.position);
          hudTarget = hudTargetMap[entity];
          hudTarget.position = hudPosition ?? new Vector2(-9999, -9999);
        }
      }
    }

  }

}
