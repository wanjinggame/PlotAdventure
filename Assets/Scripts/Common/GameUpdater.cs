using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plot.Core
{
    public class GameUpdateor : MonoBehaviour
    {
        private void Awake()
        {
            GameApp.instance.InitAllManager();
        }

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            GameApp.instance.UpdateAllManager();
        }

        private void LateUpdate()
        {
            GameApp.instance.LateUpdataManager();
        }

        private void OnApplicationFocus(bool focus)
        {
            
        }

        private void OnApplicationPause(bool pause)
        {
            
        }

        static void OnQuit()
        {
#if UNITY_EDITOR
            EditorApplication.quitting -= OnEditorQuit;
#endif
            Debug.Log("DestroyAllManager");
            GameApp.instance.DestroyAllManager();
        }

#if UNITY_EDITOR
        static void OnEditorQuit()
        {
            OnQuit();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        static void InitQuitAction()
        {
            Application.quitting += OnQuit;
#if UNITY_EDITOR
            EditorApplication.quitting += OnEditorQuit;
#endif
        }

    }
}