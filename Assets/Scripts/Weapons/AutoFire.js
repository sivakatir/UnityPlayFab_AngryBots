#pragma strict

@script RequireComponent (PerFrameRaycast)

var bulletPrefab : GameObject;
var spawnPoint : Transform;
var frequency : float = 10;
var frequencyOri : float = frequency;
var coneAngle : float = 1.5;
var coneAngleOri : float = coneAngle;
var firing : boolean = false;
var damagePerSecond : float = 20.0;
var damagePerSecondOri : float = damagePerSecond;
var forcePerSecond : float = 20.0;
var hitSoundVolume : float = 0.5;
var hitSoundVolumeOri : float = hitSoundVolume;

var muzzleFlashFront : GameObject;

private var lastFireTime : float = -1;
private var raycast : PerFrameRaycast;

function Awake () {
	muzzleFlashFront.SetActive (false);

	raycast = GetComponent.<PerFrameRaycast> ();
	if (spawnPoint == null)
		spawnPoint = transform;
}
private var gunsAmmoNames : Array = ["ammo_pack_1","ammo_pack_2"];
function Update () {
	if (firing) {
		switch(PlayFabGameBridge.currentGun){
		case 1: 
			frequency = frequencyOri;
			coneAngle = coneAngleOri;
			damagePerSecond = damagePerSecondOri;
			hitSoundVolume = hitSoundVolumeOri;
		break;
		case 2:
			frequency = 20;
			coneAngle = 5;
			damagePerSecond = 80;
			hitSoundVolume = 10;
		break;
		case 3:
			frequency = 2;
			coneAngle = 20;
			damagePerSecond = 200;
			hitSoundVolume = 5;
		break;
		}
		if (Time.time > lastFireTime + 1 / frequency) {
			// Spawn visual bullet
			var coneRandomRotation = Quaternion.Euler (Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);
			var go : GameObject = Spawner.Spawn (bulletPrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation) as GameObject;
			var bullet : SimpleBullet = go.GetComponent.<SimpleBullet> ();

			lastFireTime = Time.time;

			// Find the object hit by the raycast
			var hitInfo : RaycastHit = raycast.GetHitInfo ();
			if (hitInfo.transform) {
				// Get the health component of the target if any
				var targetHealth : Health = hitInfo.transform.GetComponent.<Health> ();
				if (targetHealth) {
					// Apply damage
					targetHealth.OnDamage (damagePerSecond / frequency, -spawnPoint.forward);
				}

				// Get the rigidbody if any
				if (hitInfo.rigidbody) {
					// Apply force to the target object at the position of the hit point
					var force : Vector3 = transform.forward * (forcePerSecond / frequency);
					hitInfo.rigidbody.AddForceAtPosition (force, hitInfo.point, ForceMode.Impulse);
				}

				// Ricochet sound
				var sound : AudioClip = MaterialImpactManager.GetBulletHitSound (hitInfo.collider.sharedMaterial);
				AudioSource.PlayClipAtPoint (sound, hitInfo.point, hitSoundVolume);

				bullet.dist = hitInfo.distance;
			}
			else 
				bullet.dist = 1000;
			if(PlayFabGameBridge.currentGun!=1)PlayFabGameBridge.consumeItem(gunsAmmoNames[PlayFabGameBridge.currentGun-2] as String);
		}
	}
}
var canShoot : boolean;
function OnStartFire () {
	canShoot = (PlayFabGameBridge.currentGun == 1 || PlayFabGameBridge.consumableItems.ContainsKey(gunsAmmoNames[PlayFabGameBridge.currentGun-2] as String));
	if (Time.timeScale == 0 || !PlayFabGameBridge.menuClosed || !canShoot)
		return;
	muzzleFlashFront.SetActive (true);
	if (audio){
	
		switch(PlayFabGameBridge.currentGun){
			case 1:audio.pitch=1;
			break;
			case 2:audio.pitch=3;
			break;
			case 3:audio.pitch=0.2;
			break;
		}
		audio.Play ();
	}
	firing = true;
		
}

function OnStopFire () {
	firing = false;

	muzzleFlashFront.SetActive (false);

	if (audio)
		audio.Stop ();
}