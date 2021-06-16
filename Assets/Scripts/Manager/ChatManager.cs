using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Networking 
{
    public class ChatManager : MonoBehaviour
    {
        private NetworkIdentity networkIdentity;
        public int maxMessages = 25;

        public GameObject chatPanel, textObject;
        public TMPro.TMP_InputField inputField;
             
        [SerializeField]
        List<Message> messageList = new List<Message>();


        void Start()
        {

        }

        void Update()
        {

            if (inputField.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendMessageToChat(inputField.text);

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

            // networkIdentity.GetSocket().Emit("chatMesage", JsonUtility.ToJson(player));
        }
    }

    [System.Serializable]
    public class Message
    {
        public string text;
        public TMPro.TextMeshProUGUI textObject;
    }

}