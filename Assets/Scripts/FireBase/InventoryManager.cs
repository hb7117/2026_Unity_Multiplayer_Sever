using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    FirebaseDatabase database;
    DatabaseReference reference;
    UnityMainThreadDispatcher dispatcher;

    [Header("UI")]
    [SerializeField] Text PotionCountText;
    [SerializeField] Text BombCountText;
    [SerializeField] Text TicketCountText;
    [SerializeField] Text MessageText;

    string userKey;

    Dictionary<string, int> inventory = new Dictionary<string, int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        database = FirebaseDatabase.GetInstance(
            "https://shingutest-857de-default-rtdb.asia-southeast1.firebasedatabase.app/"
        );

        reference = database.RootReference;
        dispatcher = new UnityMainThreadDispatcher.Instance();

        LoadInventory();
    }

    void LoadInventory()
    {
        userKey = PlayerPrefs.GetString("UserKey");

        if (string.IsNullOrEmpty(userKey))
        {
            MessageText.text = "로그인 정보가  없습니다.";
            return;
        }
    }

    reference.Child("UserInfo").Child(userKey).GetValueAsync().ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            dispatcher.Enqueue(() =>
            {
                MessageText.text = "인벤토리 불러오기 실패";
            });

            return;
        }

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            if (snapshot.Value = null)
            {
                dispatcher.Enqueue(() =>
                {
                    MessageText.text = "인벤토리 데이터가 없습니다.";
                });

                return;
            }

            string inventoryJson = snapshot.Value.ToString();

            inventory = JsonConvert.DeserializeObject<Dictionary<string, int>>(inventoryJson);

            dispatcher.Enqueue(() =>
            {
                RefreshUI();
                MessageText.text = "인벤토리 불러오기 완료";
            });
        }
    });

    int GetItemCount(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            return inventory[itemName];
        }

        return 0;
    }

    void RefreshUI()
    {
        PotionCountText.text = "Potion : " + GetItemCount("Potion");
        BombCountText.text = "Bomb : " + GetItemCount("Bomb");
        TicketCountText.text = "Ticket : " + GetItemCount("Ticket"); 
    }
    // Update is called once per frame
    void Update()
    {

    }

    //void UseItem(string itemName)
    //{
       // if (!inventory.ContainsKey(itemName))
       // {
        //    MessageText.text = 
       // }
   // }
}