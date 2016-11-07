using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

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

public class AircraftWeapon : MonoBehaviour {

    public GameObject weaponPrototype;
    public FireType fireType = FireType.Primary;
    public FireMode fireMode = FireMode.SemiAutomatic;
    public WeaponType weaponType = WeaponType.Missile;
    public GameObject target;
    public int ammo = 150;
    public int concurrency = 2;

    public float reloadSeconds = 4;
    private float[] reloadTimes;

    // public float cooldownSeconds = 0;
    // private float cooldownTime;

    private new Rigidbody rigidbody;

    public void Start() {
        reloadTimes = new float[concurrency];
        rigidbody = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	public void Update() {
        var input = GetFireInput();
        if (input) {
            StartCoroutine("FireWeapon");
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

    public IEnumerator FireWeapon() {
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
            missile.heading = rigidbody.velocity.normalized;
            missile.heading.y += 0.05f;
            missile.speed = rigidbody.velocity.magnitude + 200;
            missile.transform.position = transform.position
                + transform.forward * 10
                - transform.up * 10;
            var weapon = Instantiate(weaponPrototype);
            if (target) {
                yield return new WaitForSeconds(1f);
                weapon.GetComponent<Missile>().target = target;
            }
        }
    }

}
