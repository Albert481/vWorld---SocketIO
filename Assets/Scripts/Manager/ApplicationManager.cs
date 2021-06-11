using System.Collections;
using System.Collections.Generic;
using Project.Utility;
using UnityEngine;

namespace Project.Managers
{
    public class ApplicationManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManagementManager.Instance.LoadLevel(SceneList.MAIN_MENU, (levelName) =>
            {

            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
