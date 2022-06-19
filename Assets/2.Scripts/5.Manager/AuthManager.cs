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
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class AuthManager : Singleton<AuthManager>
{
    public bool isFireBaseReady {get;  private set;}
    public bool isSignInOnProgress {get; private set;}
    public TMP_InputField idField;
    public TMP_InputField passwordField;
    public Button signInBtn;
    public FirebaseApp firebaseApp;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser user;
    public GameObject errorTextPanel;
    public GameObject loginPanel;
 

    [Header("SignInPanel")]
    public GameObject signInPanel;
    public TMP_InputField idCreateField;
    public TMP_InputField passwordCreateField;
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }
    private void Start()
    {
        signInBtn.interactable=false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task=>
        {
            var result = task.Result;
            if(result != DependencyStatus.Available)
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
            signInBtn.interactable=isFireBaseReady;
        });
    }
    public void SignInWithEmail()
    {
        if(!isFireBaseReady ||  isSignInOnProgress || user !=null)
        {
            return;
        }
        isSignInOnProgress = true;
        signInBtn.interactable = false;

        firebaseAuth.SignInWithEmailAndPasswordAsync(idField.text,passwordField.text).
            ContinueWithOnMainThread((task=>
            {
                Debug.Log(message:$"Sign in status : {task.Status}");
                isSignInOnProgress = false;
                signInBtn.interactable=true;

                if(task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                    StartCoroutine(ErrorMessage("잘못된 입력입니다"));//task.Exception.ToString()));
                }
                else if(task.IsCanceled)
                {
                    Debug.LogError(message:"Sign-in canceled");
                }
                else
                {
                    user = task.Result;
                    StartCoroutine(ErrorMessage(user.Email+"확인"));
                    Debug.Log(user.Email);
                    SceneManager.LoadScene("NewLobbyScene");
                }

            }));
    }

    public void LogInGoogle()
    {

    }
    public void SetSignInPanel()
    {
        signInPanel.SetActive(true);
    }
    public void CreateUserId()
    {
        
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(idCreateField.text,passwordCreateField.text)
            .ContinueWith(task=>
            {
                if(task.IsCanceled)
                {
                    Debug.LogError("task Canceled");
                    return;
                }
                if(task.IsFaulted)
                {
                    StartCoroutine(ErrorMessage("제대로 된 입력이 아닙니다"));
                    Debug.Log("task is Faulted"+task.Exception);
                    
                    return;
                }

                //userCreated
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0}({1})",
                    newUser.DisplayName,newUser.UserId);
                
                signInPanel.SetActive(false);
                idCreateField.text = "";
                passwordCreateField.text = "";
                StartCoroutine(ErrorMessage("가입이 완료되었습니다"));
            });

  
    }
    IEnumerator ErrorMessage(string errorMessage)
    {
        TMP_Text error = errorTextPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        errorTextPanel.SetActive(true);
        loginPanel.SetActive(false);
        error.text = errorMessage;//errorMessage;
        yield return new WaitForSeconds(3f);
        errorTextPanel.SetActive(false);
        loginPanel.SetActive(true);
        error.text = "";
    }
}
