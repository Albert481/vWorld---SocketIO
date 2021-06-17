using System.Collections;
using System.Collections.Generic;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Networking 
{
    public class ChatManager : MonoBehaviour
    {
        public int maxMessages = 25;

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
                    Debug.Log("my networkIdentify is: " + networkIdentity.GetID());

                    SendMessageToChat(inputField.text); // sends locally
                    socketReference.SendMessage(inputField.text); // emits to socket io server


                    inputField.text = "";
                    inputField.Select();
                    inputField.ActivateInputField();
                }
            }

           
        }

        public void SendMessageToChat(string text)
        {
            if (messageList.Count >= maxMessages)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }

            Message newMessage = new Message();
            newMessage.text = text;

            GameObject newText = Instantiate(textObject, chatPanel.transform);
            newMessage.textObject = newText.GetComponent<TMPro.TextMeshProUGUI>();
            newMessage.textObject.text = newMessage.text;

            messageList.Add(newMessage);
        }
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TMPro.TextMeshProUGUI textObject;
    }

}