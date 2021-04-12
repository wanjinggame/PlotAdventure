using Plot.Core;
using Plot.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Plot.Resource
{
    public class ResourceManager : BaseManager
    {
        private List<Object> loadedObjs;

        public override void Init()
        {
            base.Init();
            loadedObjs = new List<Object>();

            if (GameLog.EnableLog(GameLog.LV_DEBUG))
            {
                GameLog.Log("ResourceManager Init Success...");
            }
        }

        public override void CommonUpdate()
        {
            base.CommonUpdate();

        }

        public override void CommonLateUpdate()
        {
            base.CommonLateUpdate();

        }

        public override void Destroy()
        {
            base.Destroy();

        }

        public T Load<T>(string resPath) where T : Object
        {

            return null;
        }
    }
}
