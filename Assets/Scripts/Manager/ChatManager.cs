using System.Collections;
using System.Collections.Generic;
using UnitySocketIO;
using UnitySocketIO.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Networking 
{
    public class ChatManager : MonoBehaviour
    {
        public int maxMessages = 25;
        public ScrollRect scrollRect;
        public GameObject chatPanel, textObject;
        public TMPro.TMP_InputField inputField;

        [SerializeField]
        private NetworkIdentity networkIdentity;

        [SerializeField]
        List<Message> messageList = new List<Message>();

        public NetworkClient socketReference;

        public NetworkClient SocketReference
        {
            get
            {
                return socketReference = (socketReference == null) ? FindObjectOfType<NetworkClient>() : socketReference;
            }
        }

        void Start()
        {
        }

        void Update()
        {

            if (inputField.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Debug.Log("my networkIdentity is: " + NetworkClient.ClientID);

                    ChatMessage newMessage = new ChatMessage();
                    newMessage.id = NetworkClient.ClientID;
                    newMessage.lobbyid = "LOBBY";
                    newMessage.message = inputField.text;

                    SendMessageToChat(newMessage); // sends locally
                    socketReference.SendMessage(newMessage); // emits to socket io server

                    inputField.text = "";
                    inputField.Select();
                    inputField.ActivateInputField();
                }
            }

           
        }

        public void SendMessageToChat(ChatMessage payload)
        {
            if (messageList.Count >= maxMessages)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }

            Debug.Log("my JObject payload: " + payload);

            Message newMessage = new Message();
            newMessage.userID = payload.id;
            newMessage.text = payload.message;

            GameObject newText = Instantiate(textObject, chatPanel.transform);
            newMessage.textObject = newText.GetComponent<TMPro.TextMeshProUGUI>();
            newMessage.textObject.text = newMessage.userID + ": " + newMessage.text;
            newMessage.textObject.fontSize = 10;

            Canvas.ForceUpdateCanvases();
            scrollRect.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();

            messageList.Add(newMessage);
        }
    }

    [System.Serializable]
    public class Message
    {
        public string userID;
        public string text;
        public TMPro.TextMeshProUGUI textObject;
    }

}