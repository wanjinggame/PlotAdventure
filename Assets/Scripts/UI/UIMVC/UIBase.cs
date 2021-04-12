using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.UI
{
    public class UIBase : MonoBehaviour
    {
        public int ID;

        private void Awake()
        {
            OnCreate();
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            OnUpdate();
        }

        private void OnDestroy()
        {
            Destroy();
        }

        protected virtual void OnCreate()
        {

        }

        protected virtual void Init()
        {

        }

        protected virtual void Destroy()
        {

        }

        protected virtual void OnUpdate()
        {

        }
    }
}

