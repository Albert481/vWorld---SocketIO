using Project.Utility.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{ 
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkTransform : MonoBehaviour
    {
        [SerializeField]
        [GreyOut]
        private Vector3 oldPosition;
        private Quaternion oldRotation;

        private NetworkIdentity networkIdentity;
        private Player player;

        private float stillCounter = 0;
        // Start is called before the first frame update
        void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            oldPosition = transform.position;
            oldRotation = transform.rotation;
            player = new Player();
            player.position = new Vector3(0, 1, 0);
            player.rotation = new Quaternion(0, 0, 0, 0);

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        public void Update()
        {
            if (networkIdentity.IsControlling())
            {
                if ((oldPosition != transform.position) || (oldRotation != transform.rotation))
                {
                    oldPosition = transform.position;
                    oldRotation = transform.rotation;
                    stillCounter = 0;
                    sendData();
                } else
                {
                    stillCounter += Time.deltaTime;
                    
                    if (stillCounter >= 1)
                    {
                        stillCounter = 0;
                        sendData();
                    }

                }
            }
        }


        private void sendData()
        {
            // Update player information
            player.position.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
            player.position.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
            player.position.z = Mathf.Round(transform.position.z * 1000.0f) / 1000.0f;

            player.rotation.x = Mathf.Round(transform.rotation.x * 1000.0f) / 1000.0f;
            player.rotation.y = Mathf.Round(transform.rotation.y * 1000.0f) / 1000.0f;
            player.rotation.z = Mathf.Round(transform.rotation.z * 1000.0f) / 1000.0f;
            player.rotation.w = Mathf.Round(transform.rotation.w * 1000.0f) / 1000.0f;

            networkIdentity.GetSocket().Emit("updatePosition", JsonUtility.ToJson(player));
            
        }
    }
}

