using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayFabInventoryMenu : PlayFabItemsController {

	public Texture2D item1,item2,item3,item1Selected,item2Selected,item3Selected;
	public int spaceInBetween;
	public bool autoUpdateConsumeItems = true;
	public int UpdateEverySeconds = 15;

	private List<Texture2D>itemTextures;
	private List<Texture2D>itemSelectedTextures;
	private Rect[] itemsRect = new Rect[4];
	private int currentItemSelected = 0;
	private int textureWidth = 1;
	private int totalWidth = 1;

	public void Start(){
		itemTextures = new List<Texture2D>(new Texture2D[] { item1,item2,item3 });
		itemSelectedTextures = new List<Texture2D>(new Texture2D[] { item1Selected,item2Selected,item3Selected });
		textureWidth = itemTextures[0].width;
		UpdateInventory ();
		if(autoUpdateConsumeItems)InvokeRepeating("ConsumeNow", 0, UpdateEverySeconds);
	}

	void OnGUI () {
		if(InventoryLoaded && PlayFabGameBridge.currentGun != null){
			totalWidth = (textureWidth * itemTextures.Count) + (spaceInBetween * itemTextures.Count - 1);
			itemsRect[0] = new Rect (Screen.width * 0.5f - totalWidth * 0.5f-itemTextures[0].width-spaceInBetween, Screen.height - itemTextures[0].height - 20, totalWidth, itemTextures[0].height - 20);

			for (int i = 1; i<=PlayFabGameBridge.gunNames.Count; i++) {
				itemsRect[i] = new Rect (itemsRect[i-1].x+spaceInBetween+itemTextures[0].width,Screen.height - itemTextures[0].height - 20,itemTextures[0].width, itemTextures[0].height);
				if (currentItemSelected == i-1) {
					itemsRect[i].y -= 10;
					GUI.DrawTexture (itemsRect[i], itemSelectedTextures[i-1]);	
				}
				else GUI.DrawTexture (itemsRect[i], itemTextures[(i-1)]);
				uint? num = 0;
				if (i > 1) // because item 1 has infinite ammo
				{
					if(PlayFabGameBridge.consumableItems.ContainsKey(PlayFabGameBridge.gunNames[i-1])){
						num = PlayFabGameBridge.consumableItems[PlayFabGameBridge.gunNames[i-1]];
					}
					Rect labelRect = GUILayoutUtility.GetRect(new GUIContent("<size=22>"+num+"</size>"), "label");
					GUI.Label (new Rect (itemsRect[i].x+itemsRect[i].width/2-labelRect.width/2, itemsRect[i].y-itemsRect[i].height+75, labelRect.width, labelRect.height), "<size=22>"+num+"</size>");
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

		if (currentItemSelected != oldItem) {
			PlayFabGameBridge.currentGunName = PlayFabGameBridge.gunNames [currentItemSelected];
			PlayFabGameBridge.currentGun = PlayFabGameBridge.gunTypes [PlayFabGameBridge.currentGunName];
		}
	}	
}