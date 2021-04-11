using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.Core
{
    public class ManagerInfo
    {
        public const int NO_TICK_INTERVAL = int.MaxValue;

        private string mgrName = string.Empty;

        public float interval;
        public float lastTick;
        public float deltaTime;
        public IManager manager;
        public bool active;

        private float fixedInterval;

        public string managerName
        {
            get
            {
                if (string.IsNullOrEmpty(mgrName))
                {
                    mgrName = manager.GetType().Name;
                }
                return mgrName;
            }
        }

        public ManagerInfo(IManager mgr, float intervalInSec)
        {
            interval = intervalInSec;
            fixedInterval = intervalInSec;
            lastTick = 0;
            active = false;
            manager = mgr;
        }

        public void UpdateInterval(float intervalInSec)
        {
            if (intervalInSec > fixedInterval)
            {
                interval = intervalInSec;
            }
            else if (intervalInSec < 0)
            {
                interval = 0;
            }
        }
    }
}
