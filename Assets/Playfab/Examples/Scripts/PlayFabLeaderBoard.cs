using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFab.Examples{
	public class PlayFabLeaderBoard : MonoBehaviour {

		public Texture2D  Icon,Background,Close,Cursor;
		public float resize;
		public int iconSpace;

		public Vector2 scrollPosition = Vector2.zero;

		private bool leaderboardLoaded = false;
		private bool showLeaderboard;
		private bool drawCursor;

		private Dictionary<string,uint> LeaderboardHighScores = new Dictionary<string,uint>() ;

		void Start () {
			leaderboardLoaded = false;
			if (PlayFabData.AuthKey != null)refreshLeaderboard ();
			else PlayFabData.LoggedIn += refreshLeaderboard;
		}

		public void refreshLeaderboard(string authKey = null) {
			PlayFab.ClientModels.GetLeaderboardRequest request = new PlayFab.ClientModels.GetLeaderboardRequest ();
			request.MaxResultsCount = 50;
			request.StatisticName = "Score";

				PlayFabClientAPI.GetLeaderboard (request, ConstructLeaderboard, OnPlayFabError);
		}

		void OnGUI () {
			if (leaderboardLoaded) {

				Rect leaderboardIconRect = new Rect (Screen.width-iconSpace -(Icon.width/1.5f),Screen.height-iconSpace-(Icon.height/1.5f),(Icon.width/1.5f),(Icon.height/1.5f) );
				if (GUI.Button (leaderboardIconRect, Icon,GUIStyle.none)) {
					showLeaderboard = !showLeaderboard;
					Time.timeScale = !showLeaderboard ? 1.0f : 0.0f;
					if (showLeaderboard)
					{
						refreshLeaderboard();
					}
				};
				drawCursor = false;
				if (Input.mousePosition.x < leaderboardIconRect.x + leaderboardIconRect.width && Input.mousePosition.x > leaderboardIconRect.x && Screen.height - Input.mousePosition.y < leaderboardIconRect.y + leaderboardIconRect.height && Screen.height - Input.mousePosition.y > leaderboardIconRect.y)
				{
					drawCursor = true;
				}

				if(showLeaderboard){
					Rect winRect = new Rect (Screen.width * 0.5f - Background.width/resize *0.5f,100,Background.width/resize,Background.height/resize);
					GUI.DrawTexture (winRect, Background);
					
					Rect closeRect = new Rect (winRect.x+winRect.width-Close.width,winRect.y,Close.width,Close.height );
					if (GUI.Button (closeRect, Close,GUIStyle.none)) {
						showLeaderboard = false;
						Time.timeScale = !showLeaderboard ? 1.0f : 0.0f;
					};

					uint inc = 0;
					scrollPosition = GUI.BeginScrollView( new Rect(winRect.x,winRect.y+40, winRect.width, 240), scrollPosition, new Rect(0, 0, winRect.width - 20, LeaderboardHighScores.Count*25));
					foreach (KeyValuePair<string, uint> PlayerScore in LeaderboardHighScores){
						GUI.Box (new Rect (10 , 25*inc, winRect.width - 35, 20),"");
						GUI.skin.label.alignment = TextAnchor.MiddleLeft;
						GUI.Label (new Rect (10+2, 25*inc, winRect.width - 43, 20), "<size=12>"+(inc+1)+"</size>");
						GUI.Label (new Rect (10+20, 25*inc, winRect.width - 43, 20), "<size=12>"+PlayerScore.Key+"</size>");
						GUI.skin.label.alignment = TextAnchor.MiddleRight;
						GUI.Label (new Rect (10+2, 25*inc, winRect.width - 43, 20), "<size=12>"+PlayerScore.Value+"</size>");
						inc++;
					}
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUI.EndScrollView();

					if (Input.mousePosition.x < winRect.x + winRect.width && Input.mousePosition.x > winRect.x && Screen.height - Input.mousePosition.y < winRect.y + winRect.height && Screen.height - Input.mousePosition.y > winRect.y)
						drawCursor = true;
				}

				if (drawCursor) {
					Rect cursorRect = new Rect (Input.mousePosition.x,Screen.height-Input.mousePosition.y,Cursor.width,Cursor.height );
					GUI.DrawTexture (cursorRect, Cursor);
					PlayFabGameBridge.mouseOverGui = true;
				}else PlayFabGameBridge.mouseOverGui = false;
			}
		}

		private void ConstructLeaderboard (PlayFab.ClientModels.GetLeaderboardResult result)
		{
			LeaderboardHighScores.Clear ();
			foreach (PlayFab.ClientModels.PlayerLeaderboardEntry entry in result.Leaderboard) {
				if (entry.DisplayName != null)
					LeaderboardHighScores.Add (entry.DisplayName, (uint)entry.StatValue); 
				else
					LeaderboardHighScores.Add (entry.PlayFabId, (uint)entry.StatValue); 
			}
			leaderboardLoaded = true;
		}

		void OnPlayFabError(PlayFabError error)
		{
			Debug.Log ("Got an error: " + error.ErrorMessage);
		}
	}
}
