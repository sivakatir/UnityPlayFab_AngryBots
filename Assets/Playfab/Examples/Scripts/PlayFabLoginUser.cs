using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFab.Examples{
	public class PlayFabLoginUser : MonoBehaviour{

		public string title = "User Login";
		public string userNameLabel = "User Name";
		public string passwordLabel = "Password";
		public string nextScene = "PF_PurchaseScene";
		public string previousScene = "PF_UserRegisterScene";
		public Texture2D playfabBackground;
		public string accountNotFound = "That account could not be found.";
		public string accountBanned = "That account has been banned.";
		public string invalidPassword = "Password is invalid (6-24 characters).";
		public string invalidUsername = "Username is invalid (3-24 characters).";
		public string wrongPassword = "Wrong password for that user.";

		private string errorLabel = "";
		private GUIStyle errorLabelStyle = new GUIStyle();

		private string userNameField = "";
		private string passwordField = "";
		private float yStart;
		private bool isPassword = true;
		private bool returnedError = false;

		private void Start (){
			errorLabelStyle.normal.textColor = Color.red;
		}

		void OnGUI () {
			if (PlayFabGameBridge.gameState == 2) {
				Rect winRect = new Rect (0,0,playfabBackground.width, playfabBackground.height);
				winRect.x = (int) ( Screen.width * 0.5f - winRect.width * 0.5f );
				winRect.y = (int) ( Screen.height * 0.5f - winRect.height * 0.5f );
				yStart = winRect.y + 80;
				GUI.DrawTexture (winRect, playfabBackground);

				if (!isPassword) {
					errorLabel = invalidPassword;
				}
				else if (!returnedError) {
					errorLabel = "";
				}

				GUI.Label (new Rect (winRect.x + 18, yStart -16, 120, 30), "<size=18>"+title+"</size>");
				GUI.Label (new Rect (winRect.x + 18, yStart+25, 120, 20), userNameLabel);
				GUI.Label (new Rect (winRect.x + 18, yStart+50, 120, 20), passwordLabel);
				GUI.Label (new Rect (winRect.x + 18, yStart+73, 120, 20), errorLabel, errorLabelStyle);
				GUI.Label (new Rect (winRect.x +18, yStart +145, 120, 20), "OR");
						
				userNameField = GUI.TextField (new Rect (winRect.x+130, yStart+25, 100, 20), userNameField);
				passwordField = GUI.PasswordField  (new Rect (winRect.x+130, yStart+50, 100, 20), passwordField,"*"[0], 20);

				if (GUI.Button (new Rect (winRect.x+18, yStart+100, 100, 30), "Login")||Event.current.Equals(Event.KeyboardEvent("[enter]"))) {
					if(userNameField.Length>0 && passwordField.Length>0)
					{
						returnedError = false;
						LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
						request.Username = userNameField;
						request.Password = passwordField;
						request.TitleId = PlayFabData.TitleId;
						PlayFabClientAPI.LoginWithPlayFab(request,OnLoginResult,OnPlayFabError);
					}
					else
					{
						isPassword = false;
					}
				}

				if (GUI.Button(new Rect(winRect.x+18, yStart+175, 120, 20),"Register"))
				{
					PlayFabGameBridge.gameState = 1;
					if(!PlayFabData.AngryBotsModActivated)Application.LoadLevel (previousScene);
				}
			}
		}
		public void OnLoginResult(LoginResult result){
			PlayFabGameBridge.gameState = 3;
			if(PlayFabData.AngryBotsModActivated)Application.LoadLevel ("Default");
			else Application.LoadLevel (nextScene);

		}
		void OnPlayFabError(PlayFabError error)
		{
			returnedError = true;
			Debug.Log ("Got an error: " + error.Error);
			if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password"))
			{
				errorLabel = invalidPassword;
			}
			else if (error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username"))
			{
				errorLabel = invalidUsername;
			}
			else if (error.Error == PlayFabErrorCode.AccountNotFound)
			{
				errorLabel = accountNotFound;
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				errorLabel = accountBanned;
			}
			else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
			{
				errorLabel = wrongPassword;
			}
			else
			{
				errorLabel = "Unknown Error.";
			}
		}




	}
}