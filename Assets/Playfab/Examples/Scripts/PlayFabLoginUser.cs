using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

namespace PlayFab.Examples{
	public class PlayFabLoginUser : MonoBehaviour{

		public string title = "Login Or Registers";
		public string userNameLabel = "User Name";
		public string passwordLabel = "Password";
		public string nextScene = "PF_PurchaseScene";
		public string previousScene = "PF_UserRegisterScene";
		public Texture2D playfabBackground,cursor;
		public string accountNotFound = "That account could not be found.";
		public string accountBanned = "That account has been banned.";
		public string invalidPassword = "Password is invalid (6-24 characters).";
		public string invalidUsername = "Username is invalid (3-24 characters).";
		public string wrongPassword = "Wrong password for that user.";
		public string loginCanceled = "Facebook login was canceled.";

		private string errorLabel = "";
		private GUIStyle errorLabelStyle = new GUIStyle();

		private string userNameField = "";
		private string passwordField = "";
		private float yStart;
		private bool isPassword = true;
		private bool returnedError = false;

		private void Start (){
			errorLabelStyle.normal.textColor = Color.red;
			FB.Init(FacebookOnInitComplete);
		}

		// if we are in "login" state, draw the login window on screen
		void OnGUI () {
			if (PlayFabGameBridge.gameState == 2) {
				if(PlayFabData.SkipLogin && PlayFabData.AuthKey != null){
					PlayFabGameBridge.gameState = 3;
					Time.timeScale = 1.0f; // unpause
				} else {
					Time.timeScale = 0.0f;	// pause everything while we show the UI

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

					// if the player clicks "login" then initiate a login request to PlayFab
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

					// if the player wants to register a new account instead, flip to the "register" dialog
					if (GUI.Button(new Rect(winRect.x+18, yStart+175, 120, 30),"Register"))
					{
						PlayFabGameBridge.gameState = 1;
						if(!PlayFabData.AngryBotsModActivated)Application.LoadLevel (previousScene);
					}

					// Facebook login only works on Facebook canvas,
					// iOS, Androind, or in the Editor where it does a dummy login.
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID
					// TODO: Cursor is BEHIND the Facebook login prompt, at least in the editor.
					if (GUI.Button(new Rect(winRect.x+18, yStart+215, 140, 30),"Login With Facebook"))
					{
						Debug.Log ("Facebook login clicked");
						if(!FB.IsLoggedIn)
						{
							FB.Login("email,publish_actions", FacebookAuthCallback); 
						}
					}
#endif

					if (Input.mousePosition.x < winRect.x + winRect.width && Input.mousePosition.x > winRect.x && Screen.height - Input.mousePosition.y < winRect.y + winRect.height && Screen.height - Input.mousePosition.y > winRect.y){
						Rect cursorRect = new Rect (Input.mousePosition.x,Screen.height-Input.mousePosition.y,cursor.width,cursor.height );
						GUI.DrawTexture (cursorRect, cursor);
					}
				}
			}
		}

		void FacebookOnInitComplete()
		{
			// Add any additional code needed for when FB.Init() is complete.
			// You can enable auto login with facebook here if desired by checking FB.IsLoggedIn == true
		}

		// facebook login callback function
		void FacebookAuthCallback(FBResult result)
		{
			if (FB.IsLoggedIn) {
				LoginWithFacebookRequest request = new LoginWithFacebookRequest();
				request.AccessToken = FB.AccessToken;
				// auto create account if one not found. setting to 
				// false for new users will generate an error.
				request.CreateAccount = true;
				request.TitleId = PlayFabData.TitleId;
				PlayFabClientAPI.LoginWithFacebook(request, OnLoginResult, OnPlayFabError);
			} else {
				errorLabel = loginCanceled;
			}
		}

		// callback function if player login is successful
		void OnLoginResult(LoginResult result){
			PlayFabGameBridge.gameState = 3;	// switch to playing the game; hide this dialog
			Time.timeScale = 1.0f;	// unpause...
			PlayFabData.AuthKey = result.SessionTicket;
			if(!PlayFabData.AngryBotsModActivated)Application.LoadLevel (nextScene);

		}

		// callback function if there is an error -- display appropriate error message
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
	    
		// Callback for Facebook Canvas login
		private void OnHideUnity(bool isGameShown)                                                   
		{                                                                                            
			Debug.Log("OnHideUnity");                                                              
			if (!isGameShown)                                                                        
			{                                                                                        
				// pause the game - we will need to hide                                             
				Time.timeScale = 0;                                                                  
			}                                                                                        
			else                                                                                     
			{                                                                                        
				// start the game back up - we're getting focus again                                
				Time.timeScale = 1;                                                                  
			}                                                                                        
		} 
	}
}