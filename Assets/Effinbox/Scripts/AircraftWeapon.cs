﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

namespace Effinbox {

  public enum FireType {
    Primary,
    Secondary,
  }

  public enum FireMode {
    SemiAutomatic,
    Automatic,
    Burst,
  }

  public enum WeaponType {
    Missile,
  }

  [RequireComponent (typeof (Rigidbody))]
  public class AircraftWeapon : MonoBehaviour {

    public GameObject weaponPrototype;
    public FireType fireType = FireType.Primary;
    public FireMode fireMode = FireMode.SemiAutomatic;
    public WeaponType weaponType = WeaponType.Missile;
    public int ammo = 150;
    public int concurrency = 2;
    public float reloadSeconds = 4;
    public bool useInput;

    private Radar radar;
    private float[] reloadTimes;
    private bool leftSide;

    // public float cooldownSeconds = 0;
    // private float cooldownTime;

    private new Rigidbody rigidbody;

    public void Start() {
      reloadTimes = new float[concurrency];
      rigidbody = GetComponent<Rigidbody>();
      radar = GetComponent<Radar>();
    }

    // Update is called once per frame
    public void Update() {
      var input = GetFireInput();
      if (input) {
        FireWeapon();
      }
    }

    private bool CanFire() {
      return GetFirableCount() > 0;
    }

    public int GetFirableCount() {
      var time = Time.time;
      var count = 0;
      foreach (var reloadTime in reloadTimes) {
        if (reloadTime < time) {
          count++;
        }
      }
      return count;
    }

    private bool GetFireInput() {
      if (!useInput) {
        return false;
      }
      if (!CanFire()) {
        return false;
      }
      if (fireType == FireType.Primary) {
        return CrossPlatformInputManager.GetButtonDown("PrimaryFire");
      }
      if (fireType == FireType.Secondary) {
        return CrossPlatformInputManager.GetButtonDown("SecondaryFire");
      }
      return false;
    }

    public void FireWeapon() {
      StartCoroutine("FireWeaponCoroutine");
    }

    private IEnumerator FireWeaponCoroutine() {
      var time = Time.time;
      var fired = false;
      for (int i = 0; i < reloadTimes.Length; i++) {
        if (reloadTimes[i] < time) {
          reloadTimes[i] = Time.time + reloadSeconds;
          fired = true;
          break;
        }
      }
      if (!fired) {
        yield break;
      }
      if (weaponType == WeaponType.Missile) {
        var missile = weaponPrototype.GetComponent<Missile>();
        missile.heading = transform.forward;
        missile.heading += transform.up * 0.04f;
        missile.speed = rigidbody.velocity.magnitude + 300;
        missile.transform.position = transform.position
          + transform.forward * 15
          - transform.up * 5;
        if (leftSide) {
          missile.heading -= transform.right * 0.02f;
          missile.transform.position -= transform.right * 5f;
          leftSide = false;
        } else {
          missile.heading += transform.right * 0.02f;
          missile.transform.position += transform.right * 5f;
          leftSide = true;
        }
        var weapon = Instantiate(weaponPrototype);
        if (radar && radar.IsLocked()) {
          var target = radar.GetSelectedTarget();
          yield return new WaitForSeconds(0.25f);
          weapon.GetComponent<Missile>().target = target.gameObject;
        }
      }
    }

  }

}
