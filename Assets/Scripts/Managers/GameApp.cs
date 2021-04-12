using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plot.Utility;
using Plot.Const;
using System;
using Plot.UI;
using Plot.Resource;

namespace Plot.Core
{
    public class GameApp : Singleton<GameApp>
    {
        private GameObject mgrHolder;
        private List<ManagerInfo> mgrInfos;
        public bool pauseGame;

        public UIManager uiManager
        {
            get; set;
        }

        public ResourceManager resourceManager
        {
            get; set;
        }

        public GameApp()
        {
            mgrInfos = new List<ManagerInfo>();
        }

        public static GameApp instance
        {
            get
            {
                return Instance();
            }
        }

        public bool CreateManagerHolder()
        {
            if (mgrHolder != null)
            {
                return false;
            }

            mgrHolder = GameObject.Find(GameConst.MANAGER_HOLDER_NAME);
            if (mgrHolder != null)
            {
                return false;
            }

            mgrHolder = new GameObject(GameConst.MANAGER_HOLDER_NAME);
            UnityEngine.Object.DontDestroyOnLoad(mgrHolder);
            return true;
        }

        public void SetupManager()
        {
            uiManager = AddManager<UIManager>(ManagerInfo.NO_TICK_INTERVAL);
            resourceManager = AddManager<ResourceManager>(ManagerInfo.NO_TICK_INTERVAL);

            mgrHolder.AddComponent<GameUpdateor>();
        }

        public void InitAllManager()
        {
            foreach (var info in mgrInfos)
            {
                try
                {
                    info.manager.Init();
                }
                catch (Exception ex)
                {
                    if (GameLog.EnableLog(GameLog.LV_DEV_ERROR))
                    {
                        GameLog.LogDevError(string.Format("InitAllManager {0} Error {1}", info.managerName, ex));
                    }
                }
            }
        }

        public void DestroyAllManager()
        {
            for (int i = mgrInfos.Count - 1; i >= 0; i--)
            {
                try
                {
                    mgrInfos[i].manager.Destroy();
                }
                catch (Exception ex)
                {
                    if (GameLog.EnableLog(GameLog.LV_DEV_ERROR))
                    {
                        GameLog.LogDevError(string.Format("DestroyAllManager  {0}  Error {1}", mgrInfos[i].managerName, ex));
                    }
                }
            }
        }

        public void UpdateAllManager()
        {
            if (pauseGame)
            {
                return;
            }
            float now = Time.unscaledTime;
            foreach (var info in mgrInfos)
            {
                float deltaTime = now - info.lastTick;
                info.active = deltaTime >= info.interval;
                if (info.active)
                {
                    info.deltaTime = deltaTime;
                    info.lastTick = now;
                }
            }

            foreach (var info in mgrInfos)
            {
                if (!info.active)
                {
                    continue;
                }

                try
                {
                    info.manager.CommonUpdate();
                }
                catch (Exception ex)
                {
                    if (GameLog.EnableLog(GameLog.LV_DEV_ERROR))
                    {
                        GameLog.LogDevError(string.Format("UpdateAllManager {0} Error {1}", info.managerName, ex));
                    }
                }
            }
        }

        public void LateUpdataManager()
        {
            if (pauseGame)
            {
                return;
            }
            foreach (var info in mgrInfos)
            {
                if (!info.active)
                {
                    continue;
                }

                try
                {
                    info.manager.CommonLateUpdate();
                }
                catch (Exception ex)
                {
                    if (GameLog.EnableLog(GameLog.LV_DEV_ERROR))
                    {
                        GameLog.LogDevError(string.Format("LateUpdateAllManager {0} Error {1}", info.managerName, ex));
                    }
                }
            }
        }

        public T AddManager<T>(float tickInterval) where T : IManager
        {
#if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                GameLog.LogError(typeof(T).Name + " is a mono manager, use [AddMonoManager] instead");
            }
#endif

            T mgr = Activator.CreateInstance<T>();
            ManagerInfo mgrInfo = new ManagerInfo(mgr, tickInterval);
            mgr.SetInfo(mgrInfo);
            mgrInfos.Add(mgrInfo);
            return mgr;
        }

        public T AddMonoManager<T>(float tickInterval, T instance = null) where T : Component, IManager
        {
            T mgr = null;
            if (instance)
            {
                mgr = instance;
            }
            else
            {
                mgr = mgrHolder.AddComponent<T>();
            }
            ManagerInfo mgrInfo = new ManagerInfo(mgr, tickInterval);
            mgr.SetInfo(mgrInfo);
            mgrInfos.Add(mgrInfo);
            return mgr;
        }
    }
}
