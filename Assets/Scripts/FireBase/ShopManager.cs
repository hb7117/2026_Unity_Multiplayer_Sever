using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ShopManager : MonoBehaviour
{
    FirebaseDatabase database;
    DatabaseReference reference;
    UnityMainThreadDispatcher dispatcher;

    [Header("UI")]
    [SerializeField] Text CoinText;
    [SerializeField] Text MessageText;

    string userKey;

    int currentCoin;
    Dictionary<string, int> inventory = new Dictionary<string, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        database = FirebaseDatabase.GetInstance(
            "https://shingutest-857de-default-rtdb.asia-southeast1.firebasedatabase.app/"
        );

        reference = database.RootReference;
        dispatcher = new UnityMainThreadDispatcher.Instance();

        LoadUserData();
    }

    public void LoadUserData()
    {
        userKey = PlayerPrefs.GetString("UserKey");

        if (string.IsNullOrEmpty(userKey))
        {
            MessageText.text = "로그인 정보가 없습니다.";
            return;
        }
    }

    reference.Child("UserInfo").Child(userKey).GetValueAsync().ContinueWith(task =>
    {
        if(task.IsFaulted)
        {
            dispatcher.Enqueue(() =>
            {
                MessageText.text = "유저 정보 불러오기 실패";
            });

            return;
        }

        if (task.Iscompleted)
        {
            DataSnapshot snapshot = task.Result;
            currentCoin = int.Parse(snapshot.Child("Coin").Value.ToString());
        }
    };

    void RefreshUI()
    {
        CoinText.text = "Coin : " + currentCoin;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
