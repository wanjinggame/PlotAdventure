using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.Core
{
    public class BaseManager : IManager
    {
        private ManagerInfo mgrInfo;

        public virtual void CommonUpdate()
        {
        }

        public virtual void CommonLateUpdate()
        {
        }

        public virtual void Destroy()
        {
        }

        public virtual void Init()
        {
        }

        public float deltaTime
        {
            get
            {
                return mgrInfo.deltaTime;
            }
        }

        public virtual void SetFPS(int fps)
        {
            float interVal = 0.9f / fps;
            mgrInfo.UpdateInterval(interVal);
        }

        public virtual void SetInfo(ManagerInfo info)
        {
            mgrInfo = info;
        }
    }
}
