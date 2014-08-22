#pragma strict
#pragma downcast

var checkpoint : Transform;

function OnSignal () {
	ResetHealthOnAll ();
	transform.position = checkpoint.position;
	transform.rotation = checkpoint.rotation;
}

static function ResetHealthOnAll () {
	var healthObjects : Health[] = FindObjectsOfType (Health);
	for (var health : Health in healthObjects) {
		health.enabled = true;
		health.dead = false;
		health.health = health.maxHealth;
		PlayFabGameBridge.playerHealth = Mathf.Ceil(health.health);
	}	
}	
