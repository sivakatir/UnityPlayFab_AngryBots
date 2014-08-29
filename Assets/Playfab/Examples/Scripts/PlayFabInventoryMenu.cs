using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;

public class PlayFabInventoryMenu : PlayFabItemsController {

	public Texture2D item1,item2,item3,item4,item1Selected,item2Selected,item3Selected,item4Selected;
	public int spaceInBetween;
	public bool autoUpdateConsumeItems = true;
	public int UpdateEverySeconds = 15;

	private Dictionary<string,Texture2D>itemTextures;
	private Dictionary<string,Texture2D>itemSelectedTextures;
	private Rect[] itemsRect = new Rect[5];
	private int currentItemSelected = 0;
	private int textureWidth = 1;
	private int totalWidth = 1;
	private bool healthItemPressed = false;

	public void Start(){
		itemTextures = new Dictionary<string,Texture2D> ();
		itemTextures.Add ("Default", item1);
		itemTextures.Add ("AmmoPack:Burst", item2);
		itemTextures.Add ("AmmoPack:Cannon", item3);
		itemTextures.Add ("HealthPack:Partial", item4);

		itemSelectedTextures = new Dictionary<string,Texture2D> ();
		itemSelectedTextures.Add ("Default", item1Selected);
		itemSelectedTextures.Add ("AmmoPack:Burst", item2Selected);
		itemSelectedTextures.Add ("AmmoPack:Cannon", item3Selected);
		itemSelectedTextures.Add ("HealthPack:Partial", item4Selected);

		textureWidth = itemTextures["Default"].width;
		if (PlayFabData.AuthKey != null) UpdateInventory ();
		else PlayFabData.LoggedIn += UpdateInventory;
		if(autoUpdateConsumeItems)InvokeRepeating("ConsumeNow", 0, UpdateEverySeconds);
	}

	void OnGUI () {
		if(InventoryLoaded && PlayFabGameBridge.gunNames != null && PlayFabGameBridge.currentGun != null && PlayFabGameBridge.gameState == 3){
			totalWidth = (textureWidth * PlayFabGameBridge.itemNames.Count) + (spaceInBetween * PlayFabGameBridge.itemNames.Count - 1);
			int inc = 0;
			foreach(KeyValuePair<string, Texture2D> entry in itemTextures)
			{
				if(PlayFabGameBridge.itemNames.Contains(entry.Key)){
					itemsRect[inc] = new Rect ((Screen.width/2) -( totalWidth/2) + (spaceInBetween*inc) + (itemTextures[entry.Key].width *inc),Screen.height - itemTextures[entry.Key].height - 20,itemTextures[entry.Key].width, itemTextures[entry.Key].height);
					if (currentItemSelected == inc || healthItemPressed && PlayFabGameBridge.consumableItems.ContainsKey("HealthPack:Partial") && entry.Key == "HealthPack:Partial") {
						itemsRect[inc].y -= 10;
						GUI.DrawTexture (itemsRect[inc], itemSelectedTextures[entry.Key]);	
					}
					else GUI.DrawTexture (itemsRect[inc], entry.Value);

					if (inc >0) // because item 1 has infinite ammo
					{
						uint? num = 0;
						if(PlayFabGameBridge.consumableItems.ContainsKey(entry.Key)){
							num = PlayFabGameBridge.consumableItems[entry.Key];
						}
						Rect labelRect = GUILayoutUtility.GetRect(new GUIContent("<size=22>"+num+"</size>"), "label");
						GUI.Label (new Rect (itemsRect[inc].x+(itemsRect[inc].width/2)-(labelRect.width/2), itemsRect[inc].y-itemsRect[0].height+75, labelRect.width, labelRect.height), "<size=22>"+num+"</size>");
					}
					inc++;
				}
			}
		}
	}
	private void ConsumeNow(){
		ConsumeItems ();
	}
	void Update(){
		var oldItem = currentItemSelected;

		if (Input.GetKeyDown (KeyCode.Alpha1))currentItemSelected=0;
		if (Input.GetKeyDown (KeyCode.Alpha2))currentItemSelected=1;
		if (Input.GetKeyDown (KeyCode.Alpha3))currentItemSelected=2;
		if (Input.GetKeyDown (KeyCode.H) && !healthItemPressed) {
			healthItemPressed = true;
			PlayFabGameBridge.consumeItem("HealthPack:Partial");
			if(PlayFabGameBridge.playerHealth<=90)
				PlayFabGameBridge.playerHealth += 10;
			else
				PlayFabGameBridge.playerHealth += (100 - PlayFabGameBridge.playerHealth);
	
		}
		if (Input.GetKeyUp (KeyCode.H))
						healthItemPressed = false;

		if (currentItemSelected != oldItem) {
			PlayFabGameBridge.currentGunName = PlayFabGameBridge.gunNames [currentItemSelected];
			PlayFabGameBridge.currentGun = PlayFabGameBridge.gunTypes [PlayFabGameBridge.currentGunName];
		}
	}	
}