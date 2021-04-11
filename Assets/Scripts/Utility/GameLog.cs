using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.Utility
{
    public static class GameLog
    {
        public static bool enabled = true;

        public const int LV_DEBUG = 1;
        public const int LV_WARN = 2;
        public const int LV_ERROR = 3;
        public const int LV_NONE = 4;
#if GAME_RELEASE
        public const int LV_DEV_ERROR = LV_WARN;
#else
        public const int LV_DEV_ERROR = LV_ERROR;
#endif
        private static int logLevel = LV_DEBUG;
        private static int logLevelGuard = LV_DEBUG;

        public static void Log(object msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.Log(msg);
        }

        public static void RLog(string msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.Log(string.Format("<color=#ff0000>{0}</color>", msg));
        }

        public static void GLog(string msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.Log(string.Format("<color=#00ff00>{0}</color>", msg));
        }

        public static void Log(string tag, string msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.unityLogger.Log(tag, msg);
        }

        public static void DLog(params object[] args)
        {
            if (!enabled)
            {
                return;
            }
            if (args == null)
            {
                Debug.Log(null);
                return;
            }
            string msg = "";
            for (int i = 0; i < args.Length; ++i)
            {
                string str;
                if (args[i] == null)
                {
                    str = "null";
                }
                else
                {
                    str = args[i].ToString();
                }
                if (i == 0)
                {
                    msg += str;
                }
                else
                {
                    msg += "    " + str;
                }
            }
            Debug.Log(msg);

        }

        public static void MuteLog(bool mute)
        {
            if (mute)
            {
                logLevel = LV_NONE;
            }
            else
            {
                logLevel = logLevelGuard;
            }
        }

        public static void LogError(object msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.LogError(msg);
        }

        public static void LogError(string tag, object msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.unityLogger.LogError(tag, msg);
        }

        public static void LogDevError(object msg)
        {
            if (!enabled)
            {
                return;
            }
#if PG_RELEASE
            Debug.LogWarning(msg);
#else
            Debug.LogError(msg);
#endif
        }

        public static void LogDevError(string tag, object msg)
        {
            if (!enabled)
            {
                return;
            }
#if PG_RELEASE
            Debug.unityLogger.LogWarning(tag, msg);
#else
            Debug.unityLogger.LogError(tag, msg);
#endif
        }

        public static void LogException(Exception exc)
        {
            if (!enabled)
            {
                return;
            }

            Debug.LogException(exc);
        }

        public static void LogWarning(object msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.LogWarning(msg);
        }

        public static void LogWarning(string tag, object msg)
        {
            if (!enabled)
            {
                return;
            }

            Debug.unityLogger.LogWarning(tag, msg);
        }

        public static void UpdateLogLevel(bool enableLogBelowError)
        {
            if (enableLogBelowError)
            {
                logLevel = LV_DEBUG;
            }
            else
            {
                logLevel = LV_ERROR;
            }

            logLevelGuard = logLevel;
        }

        public static bool EnableLog(int lv)
        {
            return lv >= logLevel;
        }
    }
}

