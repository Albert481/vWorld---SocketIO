using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Project.Utility;

namespace Project.Networking
{
    public class NetworkClient : MonoBehaviour
    {
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefab;

        public SocketIOController io;

        public static string ClientID { get; private set; }

        private Dictionary<string, NetworkIdentity> serverObjects;

        // Start is called before the first frame update
        public void Start()
        {
            // base.Start();
            initialize();
            setupEvents();
            io.Connect();
        }

        // Update is called once per frame
        public void Update()
        {
            // base.Update();
        }
        private void initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void setupEvents()
        {
            io.On("connect", (SocketIOEvent E) =>
            {
                Debug.Log("Connection made to the server.");
            });

            io.On("registerPlayer", (SocketIOEvent E) =>
            {
                // ClientID = E.data["id"].ToString().RemoveQuotes();
                var obj = (JObject) JsonConvert.DeserializeObject<object>(E.data);
                ClientID = obj["id"].Value<string>();

                Debug.LogFormat("Client ID ({0})", ClientID);
            });

            io.On("spawnPlayer", (SocketIOEvent E) =>
            {
                // Handling all spawning all players
                // Passed Data
                // string id = E.data["id"].ToString().RemoveQuotes();
                var obj = (JObject)JsonConvert.DeserializeObject<object>(E.data);
                string id = obj["id"].Value<string>();

                GameObject go = Instantiate(playerPrefab, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.setControllerID(id);
                ni.SetSocketReference(io);
                serverObjects.Add(id, ni);

                if (ni.IsControlling())
                {
                    Camera.main.GetComponentInChildren<CameraFollow>().setTarget(go.transform);
                }

            });

            io.On("loadGame", (SocketIOEvent E) =>
            {
                Debug.Log("Switching to game");
                SceneManagementManager.Instance.LoadLevel(SceneList.LEVEL, (levelName) =>
                {
                    SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
                });
            });

            io.On("updatePosition", (SocketIOEvent E) =>
            {
                // string id = E.data["id"].ToString().RemoveQuotes();
                var obj = (JObject)JsonConvert.DeserializeObject<object>(E.data);
                string id = obj["id"].Value<string>();

                float xPosition = obj["position"]["x"].Value<float>();
                float yPosition = obj["position"]["y"].Value<float>();
                float zPosition = obj["position"]["z"].Value<float>();

                float xRotation = obj["rotation"]["x"].Value<float>();
                float yRotation = obj["rotation"]["y"].Value<float>();
                float zRotation = obj["rotation"]["z"].Value<float>();
                float wRotation = obj["rotation"]["w"].Value<float>();

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(xPosition, yPosition, zPosition);


                ni.transform.rotation = new Quaternion(xRotation, yRotation, zRotation, wRotation);
            });

            io.On("disconnected", (SocketIOEvent E) =>
            {
                // string id = E.data["id"].ToString().RemoveQuotes();
                var obj = (JObject)JsonConvert.DeserializeObject<object>(E.data);
                string id = obj["id"].Value<string>();

                GameObject go = serverObjects[id].gameObject;
                Destroy(go); // Remove from game
                serverObjects.Remove(id); // Remove from memory
            });
        }

        public void AttemptToJoinLobby()
        {
            io.Emit("joinGame");
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

