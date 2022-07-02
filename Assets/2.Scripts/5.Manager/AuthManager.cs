using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class AuthManager : Singleton<AuthManager>
{
    public AuthLoginPanel loginPanel;
    public AuthSignInPanel signInPanel;
    public bool isFireBaseReady {get;  private set;}
    public bool isSignInOnProgress {get; private set;}

    public FirebaseApp firebaseApp;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser user;
    public GameObject errorTextPanel;
 
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }
    private void Start()
    {
        CheckAvailableFirebase();
    }
    public void CheckAvailableFirebase()
    {
        loginPanel.signInBtn.interactable = false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var result = task.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.LogError(message: result.ToString());
                isFireBaseReady = false;
            }
            else
            {
                isFireBaseReady = true;
                firebaseApp = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            loginPanel.signInBtn.interactable = isFireBaseReady;
        });
    }
    public void LogInWithEmail()
    {
        if(!isFireBaseReady ||  isSignInOnProgress || user !=null)
        {
            return;
        }

        isSignInOnProgress = true;
        loginPanel.signInBtn.interactable = false;

        firebaseAuth.SignInWithEmailAndPasswordAsync(loginPanel.idField.text,loginPanel.passwordField.text).
            ContinueWithOnMainThread((task=>
            {
                isSignInOnProgress = false;
                loginPanel.signInBtn.interactable=true;


                if(task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                    StartCoroutine(ErrorMessage("잘못된 입력입니다. ID와 비밀번호를 확인해주세요"));//task.Exception.ToString()));
                }
                else if(task.IsCanceled)
                {
                    Debug.LogError(message:"Sign-in canceled");
                }
                else
                {
                    user = task.Result;
                    DBManager.isLoginEmail=true;
                    DBManager.Instance.GetUserID(loginPanel.idField.text,(str)=>{
                        DBManager.userID = str;
                        LobbyManager.instance.loginPanel.NickNameSet();
                        
                    });
                    StartCoroutine(ErrorMessage(user.Email+"확인"));

                    ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable() { { GameData.IS_EMAIL, true } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
                    Debug.Log(user.Email);
                    
                    SceneManager.LoadScene("NewLobbyScene");
                }

            }));
    }

    public void LogInAnonymously()
    {
        if(!isFireBaseReady ||  isSignInOnProgress || user !=null)
        {
            return;
        }
        isSignInOnProgress = true;
        loginPanel.signInBtn.interactable = false;

        firebaseAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task => {

            isSignInOnProgress = false;
            loginPanel.signInBtn.interactable=true;

            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            else if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            else
            {
                ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable() { { GameData.IS_EMAIL, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

                SceneManager.LoadScene("NewLobbyScene");
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            }


        });
    }

    public void NickNameCheck()
    {
        if(signInPanel.passwordCreateField.text.ToString() != signInPanel.passwordCreateFieldConfirm.text.ToString())
        {
            StartCoroutine(ErrorMessage("비밀번호를 다시 확인해주세요"));
             return;
        }

            DBManager.Instance.NickNameDuplicateCheck(signInPanel.nickNameField.text,(str)=>{
                if(str == "nullString")
                {
                    CreateUserId();
                }
                else
                {
                    StartCoroutine(ErrorMessage("이미 사용중인 닉네임입니다"));
                }
            });
    }

    public void CreateUserId()
    {

        firebaseAuth.CreateUserWithEmailAndPasswordAsync(signInPanel.idCreateField.text,signInPanel.passwordCreateField.text)
            .ContinueWithOnMainThread(task=>
            {
                if(task.IsCanceled)
                {
                    Debug.LogError("task Canceled");
                    return;
                }
                else if(task.IsFaulted)
                {
                    StartCoroutine(ErrorMessage("제대로 된 입력이 아닙니다"));
                    Debug.Log("task is Faulted"+task.Exception);

                    return;
                }
                else//성공
                {
                    //userCreated
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("Firebase user created successfully: {0}({1})",
                        newUser.DisplayName,newUser.UserId);
                    
                    DBManager.Instance.WriteNewPlayerDB(newUser.UserId,signInPanel.idCreateField.text,signInPanel.nickNameField.text);
                    
                    signInPanel.signInPanel.SetActive(false);

                    StartCoroutine(ErrorMessage("가입이 완료되었습니다"));
                    loginPanel.idField.text = signInPanel.idCreateField.text;

                    signInPanel.nickNameField.text = "";
                    signInPanel.idCreateField.text = "";
                    signInPanel.passwordCreateField.text = "";
                }

            });
    }

    public void SetSignInPanel()
    {
        signInPanel.signInPanel.SetActive(true);
        signInPanel.nickNameField.text = "";
        loginPanel.idField.text = "";
        loginPanel.passwordField.text = "";
    }
    public void CancelOnCreateIDPanel()
    {
        signInPanel.signInPanel.SetActive(false);
    }
    public IEnumerator ErrorMessage(string errorMessage)
    {
        TMP_Text error = errorTextPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        errorTextPanel.SetActive(true);
        loginPanel.loginPanel.SetActive(false);
        signInPanel.createIDBtn.interactable=false;
        signInPanel.cancelSignInPanelBtn.interactable=false;

        error.text = errorMessage;//errorMessage;
        yield return new WaitForSeconds(2.5f);
        errorTextPanel.SetActive(false);
        loginPanel.loginPanel.SetActive(true);
        signInPanel.createIDBtn.interactable=true;
        signInPanel.cancelSignInPanelBtn.interactable=true;
        error.text = "";
    }
}
