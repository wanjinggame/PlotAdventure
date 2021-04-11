using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.Core
{
    public class GameInit : MonoBehaviour
    {
        public bool isInitScene = false;

        private void Awake()
        {
            if (!isInitScene)
            {
                return;
            }
            GameApp.instance.CreateManagerHolder();
            GameApp.instance.SetupManager();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

