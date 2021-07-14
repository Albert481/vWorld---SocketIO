using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using UnitySocketIO;
using UnitySocketIO.Events;

using UnityEngine;
using UnityEngine.UI;

namespace Project.Managers
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Join Now")]
        [SerializeField]
        private GameObject joinContainer;

        [SerializeField]
        private Button queueButton;


        [Header("Sign In")]
        [SerializeField]
        private GameObject signInContainer;

        [Header("Chat")]
        [SerializeField]
        private GameObject chatContainer;

        private string username;
        private string password;

        public NetworkClient socketReference;

        public NetworkClient SocketReference
        {
            get
            {
                return socketReference = (socketReference == null) ? FindObjectOfType<NetworkClient>() : socketReference;
            }
        }

        public void Start()
        {
            queueButton.interactable = false;
            signInContainer.SetActive(false);
            joinContainer.SetActive(false);
        }

        public void OnQueue()
        {
            socketReference.AttemptToJoinLobby();
        }

        public void EditUsername(string text)
        {
            username = text;
        }

        public void EditPassword(string text)
        {
            password = text;
        }

    }

}
