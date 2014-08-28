using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFab.Examples{
	public class PlayFabTitleData : MonoBehaviour {

		public static bool PlayFabTitleDataLoaded = false;
		public static Dictionary<string,string> Data ;

		void Start () {
			if (PlayFabData.AuthKey != null)LoadTitleData ();
			else PlayFabData.LoggedIn += LoadTitleData;

		}

		private void LoadTitleData(string authKey = null){
			GetTitleDataRequest request = new GetTitleDataRequest ();
			List<string> keys = new List<string> ();
			keys.Add ("connectionlost");
			keys.Add ("nocoins");
			keys.Add ("icon_currency");
			keys.Add ("icon_health");
			keys.Add ("icon_kill");
			request.Keys = keys;
			PlayFabClientAPI.GetTitleData (request, OnTitleData, OnPlayFabError);

		}
		private void OnTitleData( GetTitleDataResult result){
			Data = result.Data;
			PlayFabTitleDataLoaded = true;
		}

		void OnPlayFabError(PlayFabError error)
		{
			Debug.Log ("Got an error: " + error.ErrorMessage);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
