using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		// Use this for initialization
		void Start () {
			LeaderboardHighScores.Add ("adhht",90); 
			LeaderboardHighScores.Add ("ueryetyu",89); 
			LeaderboardHighScores.Add ("qewyqey",88); 
			LeaderboardHighScores.Add ("ddarhddd",50); 
			LeaderboardHighScores.Add ("eewetheee dgld",49); 
			LeaderboardHighScores.Add ("arhadh",47); 
			LeaderboardHighScores.Add ("byeryj",30); 
			LeaderboardHighScores.Add ("kfjlcl",39); 
			LeaderboardHighScores.Add ("fjlfhl dfgjlfhj",37); 
			LeaderboardHighScores.Add ("adasdfhht",90); 
			LeaderboardHighScores.Add ("uexvb rasdfyetyu",89); 
			LeaderboardHighScores.Add ("qeasdfwyqey",88); 
			LeaderboardHighScores.Add ("da bb xvvsdfdarhddd",50); 
			LeaderboardHighScores.Add ("eeasdfwetheee dgld",49); 
			LeaderboardHighScores.Add ("arab sdghadh",47); 
			LeaderboardHighScores.Add ("byfgb beryj",30); 
			LeaderboardHighScores.Add ("kfxsxvb dxvgbjlcl",39); 
			LeaderboardHighScores.Add ("fjfxb gblfhl dfgjlfhj",37); 
			LeaderboardHighScores.Add ("adfxb dnhht",90); 
			LeaderboardHighScores.Add ("ufdg eryetyu",89); 
			LeaderboardHighScores.Add ("qedcx fgnwyqey",88); 
			LeaderboardHighScores.Add ("dddfgnarhddd",50); 
			LeaderboardHighScores.Add ("eeadfwetheee dgld",49); 
			LeaderboardHighScores.Add ("ab rhadh",47); 
			LeaderboardHighScores.Add ("bybxc eryj",30); 
			LeaderboardHighScores.Add ("kfjdaflcl",39); 
			LeaderboardHighScores.Add ("fjcxblfhl dfgjlfhj",37); 
			leaderboardLoaded = true;
		}
		
		// Update is called once per frame
		void OnGUI () {
			if (leaderboardLoaded) {

				Rect leaderboardIconRect = new Rect (Screen.width-iconSpace -Icon.width,Screen.height-iconSpace-Icon.height,Icon.width,Icon.height );
				if (GUI.Button (leaderboardIconRect, Icon,GUIStyle.none)) {
					showLeaderboard = !showLeaderboard;
					Time.timeScale = !showLeaderboard ? 1.0f : 0.0f;
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
						GUI.Label (new Rect (10+2, 25*inc, winRect.width - 43, 20), "<size=12>"+PlayerScore.Key+"</size>");
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
				}
			}

		}
	}
}
