using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{
    public class Nametag : MonoBehaviour
    {

        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        void Start()
        {
            SetName();
        }

        private void SetName()
        {
            nameText.text = NetworkClient.ClientID;
        }
    }

}
