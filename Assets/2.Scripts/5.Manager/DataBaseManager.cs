using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using System;

public class DataBaseManager : Singleton<DataBaseManager>
{
    [Header("정적변수")]
    [HideInInspector]
    public static bool isLoginEmail=false;
    [HideInInspector]
    public static string userID=null;

    string DBURL = "https://unity-team-project-trim-default-rtdb.firebaseio.com/";

    string cacheString=null;

    

    private bool IsTaskFinished;
    public bool isTaskFinished
    {
        get 
        {
            return IsTaskFinished;
        }
        set
        {
            IsTaskFinished = value;
            testFunc();
        } 
    }
    DatabaseReference reference;
    private void Awake()
    {
        if (_instance == null) _instance = this;
        
    }
    private void Start()
    {
        
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBURL);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    
        //ReadDB("awIhaxye4IOqouz6XCJZMjcvWmB2","emailID");
        // ReadDB("awIhaxye4IOqouz6XCJZMjcvWmB2","emailID", OnDone);
        // ReadDB("awIhaxye4IOqouz6XCJZMjcvWmB2","emailID", (str) =>{
        //     Debug.Log(str);
        // });
    }

    public void OnDone(string result)
    {
        Debug.Log(result);
    }

    public void testFunc()
    {
        Debug.Log(cacheString);
    }
    public void WriteDB(string UID,string emailID, string nickName)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        UserData data1 = new UserData(emailID,nickName);
        string jsonData1 = JsonUtility.ToJson(data1);

        reference.Child("UserData").Child(UID).SetRawJsonValueAsync(jsonData1);
    }


    public void ReadDB(string UID , string whatToFind, UnityAction<string> onDone)
    {
        //StartCoroutine(ExecuteReadDB());
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserData");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    if(data.Key == UID)
                    {
                        IDictionary dicUserData = (IDictionary)data.Value;
                        Debug.Log("email : " + dicUserData["emailID"] + "/ nickName : " +dicUserData["nickName"]);
                        cacheString = (dicUserData[whatToFind]).ToString();  
                        Debug.Log("찾는값찾는값 : "+cacheString);          
                    }
                    else
                    {
                        Debug.Log("값이 없습니다. 확인해주세요");
                    }
                }
                //isTaskFinished = true;
                onDone?.Invoke(cacheString);
            }
        });
    }

    public void GetUserID(string emailID, UnityAction<string> onDone)
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserData");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    IDictionary dicUserData = (IDictionary)data.Value;
                    if(dicUserData["emailID"].ToString() == emailID)
                    {
                        onDone?.Invoke(data.Key);
                    }
                }               
            }
        });
    }
    public class UserData
    {
        //public string
        public string emailID; 
        public string nickName;
        public UserData(string emailID, string nickName)
        {
            this.emailID = emailID;
            this.nickName = nickName;
        }
    }

    // IEnumerator ExecuteReadDB()
    // {
    //     yield return new WaitUntil(()=>isTaskFinished);
    //     //cacheString;
    //     isTaskFinished = false;
    // }


#region test
        public void WriteNickName(string emailID, string nickName)
    {
        //UserNickName name = new UserNickName(nickName);
        //string jsonData = JsonUtility.ToJson(name);
        //reference.Child("Users").Child(emailID).Child("username").SetValueAsync(nickName);
        //ContinueWith(task=>
        // {
        //     if(task.IsCompleted)
        //     {
        //         Debug.Log("saveSuccess");
        //     }
        //     else
        //     {
        //         Debug.Log("saveNotSuccess");
        //     }
        // });
        String value = emailID;
        string[] words = value.Split('.');
        value = words[0];
        reference.Child("USERS").Child(value).Child("UserNickname").SetValueAsync(nickName);
        //reference.Child("useruser").SetValueAsync(nickName);
    }
    public string ReadNickName(string emailID)
    {
        String key = emailID;
        string[] words = key.Split('.');
        key = words[0];
        Debug.Log(key);
        string returnValue=null;
        
        reference = FirebaseDatabase.DefaultInstance.RootReference;
       // var getValue = reference.EqualTo(value);
       // Query query;

        reference.Child(key)
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
            
                //Debug.Log(snapshot.Child(key).Child("UserNickname").Value);
                Debug.Log((snapshot.Child("UserNickName").Value).ToString());
            }
        });
        // FirebaseDatabase.DefaultInstance.GetReference("USERS").GetValueAsync().ContinueWithOnMainThread(task=>
        // {
        //     if(task.IsFaulted)
        //     {

        //     }
        //     else if(task.IsCompleted)
        //     {
        //         DataSnapshot snapshot = task.Result;
        //         for(int i=0; i<snapshot.ChildrenCount;++i)
        //         {
        //             snapshot.GetRawJsonValue().ToString();
        //         }
        //     }
        // });
        // reference.Child(key).GetValueAsync().ContinueWithOnMainThread(task=>
        // {
        //     if(task.IsCompleted)
        //     {
        //         Debug.Log("성공");
        //         DataSnapshot snapshot = task.Result;
        //         returnValue = snapshot.Value.ToString();
        //     }
        //     else
        //     {
        //         AuthManager.Instance.StartCoroutine(AuthManager.Instance.ErrorMessage("별명을 불러오지 못했습니다"));
                
        //     }
        // });
        
        
        return returnValue;
    }
#endregion
}
