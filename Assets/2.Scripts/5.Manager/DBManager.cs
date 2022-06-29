using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using System;

public class DBManager : Singleton<DBManager>
{
    [Header("정적변수")]
    [HideInInspector]
    public static bool isLoginEmail=false;
    [HideInInspector]
    public static string userID=null;

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
        } 
    }
    DatabaseReference reference;
    private void Awake()
    {
        if (_instance == null) _instance = this;
        
    }
    public void WriteNewPlayerDB(string UID,string emailID, string nickName)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        UserData data1 = new UserData(emailID,nickName,0.ToString(),0.ToString());
        string jsonData1 = JsonUtility.ToJson(data1);

        reference.Child("UserDatabase").Child(UID).SetRawJsonValueAsync(jsonData1);
    }
    public void WriteExistingPlayerDB(string UID,string emailID, string nickName,string totalGame, string winGames)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        UserData data1 = new UserData(emailID,nickName,totalGame,winGames);
        string jsonData1 = JsonUtility.ToJson(data1);

        reference.Child("UserDatabase").Child(UID).SetRawJsonValueAsync(jsonData1);
    }
    public void NickNameDuplicateCheck(string nickName, UnityAction<string> onDone) 
    //새로 계정 생성시, 닉네임 중복체크함수
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserDatabase");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            int childrenCount=0;
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    IDictionary dicUserData = (IDictionary)data.Value;
                    if(dicUserData["nickName"].ToString() == nickName)
                    {
                        onDone?.Invoke(data.Key);
                    }
                    else
                    {
                        ++childrenCount;
                        if(childrenCount == snapshot.ChildrenCount)
                        {
                            onDone?.Invoke("nullString");
                        }
                    }
                }               
            }
        });
    }


    public void ReadDB(string UID , string whatToFind, UnityAction<string> onDone)
    {
        //StartCoroutine(ExecuteReadDB());
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserDatabase");
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

    public void ReadPlayerInfo(string UID , bool withMail, UnityAction<string> onDone)
    {
        string returnString=null;
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserDatabase");
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
                        if(withMail)
                        {
                            returnString =
                                (dicUserData["emailID"].ToString() +"$"+
                                dicUserData["nickName"].ToString() +"$"+ 
                                dicUserData["totalGames"].ToString() +"$"+
                                dicUserData["winGames"].ToString());                           
                        }
                        else
                        {
                            returnString =
                                (dicUserData["nickName"].ToString() +"$"+ 
                                dicUserData["totalGames"].ToString() +"$"+
                                dicUserData["winGames"].ToString());
                        }
     

                    }
                    else
                    {
                        Debug.Log("값이 없습니다. 확인해주세요");
                    }
                }
                onDone?.Invoke(returnString);
            }
        });
    }

    public void GetUserID(string emailID, UnityAction<string> onDone)
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserDatabase");
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
    public void GetUserIDbyNickName(string nickName, UnityAction<string> onDone)
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("UserDatabase");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            int childrenCount=0;
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    IDictionary dicUserData = (IDictionary)data.Value;
                    if(dicUserData["nickName"].ToString() == nickName)
                    {
                        onDone?.Invoke(data.Key);
                    }
                    else
                    {
                        ++childrenCount;
                        if(childrenCount == snapshot.ChildrenCount)
                        {
                            onDone?.Invoke("nullString");
                        }
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
        public string totalGames;
        public string winGames;

        public UserData(string emailID, string nickName, string totalGames, string winGames)
        {
            this.emailID = emailID;
            this.nickName = nickName;
            this.totalGames = totalGames;
            this.winGames = winGames;
        }
    }

}
