using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using Project.Utility;

namespace Project.Networking
{
    public class NetworkClient : SocketIOComponent
    {
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefab;

        public static string ClientID { get; private set; }

        private Dictionary<string, NetworkIdentity> serverObjects;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            initialize();
            setupEvents();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
        private void initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void setupEvents()
        {
            On("open", (E) =>
            {
                Debug.Log("Connection made to the server.");
            });

            On("registerPlayer", (E) =>
            {
                ClientID = E.data["id"].ToString().RemoveQuotes();
                Debug.LogFormat("Client ID ({0})", ClientID);
            });

            On("spawnPlayer", (E) =>
            {
                // Handling all spawning all players
                // Passed Data
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = Instantiate(playerPrefab, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.setControllerID(id);
                ni.SetSocketReference(this);
                serverObjects.Add(id, ni);

                Camera.main.GetComponent<CameraFollow>().setTarget(go.transform);

            });

            On("updatePosition", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();
                float xPosition = E.data["position"]["x"].f;
                float yPosition = E.data["position"]["y"].f;
                float zPosition = E.data["position"]["z"].f;

                float xRotation = E.data["rotation"]["x"].f;
                float yRotation = E.data["rotation"]["y"].f;
                float zRotation = E.data["rotation"]["z"].f;
                float wRotation = E.data["rotation"]["w"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(xPosition, yPosition, zPosition);


                ni.transform.rotation = new Quaternion(xRotation, yRotation, zRotation, wRotation);
            });

            On("disconnected", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = serverObjects[id].gameObject;
                Destroy(go); // Remove from game
                serverObjects.Remove(id); // Remove from memory
            });
        }

        public void AttemptToJoinLobby()
        {
            Emit("joinGame");
        }

    }

    [Serializable]
    public class Player
    {
        public string id;
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }
}

