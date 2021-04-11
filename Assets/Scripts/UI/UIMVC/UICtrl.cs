using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.UI
{
    public class UICtrl<V, M> : UIBase where V : UIView, new() where M : UIModel, new()
    {
        public UIModel model;
        public UIView view;

        private void Awake()
        {
            Create();
        }

        void Create()
        {
            model = new V() as UIModel;
            view = new M() as UIView;

            OnCreate();
            view.OnCreate(transform);
            model.OnCreate();
        }

        // Start is called before the first frame update
        void Start()
        {
            view.FindObject();
            view.InitView();
            model.Init();
            Init();
        }

        protected virtual void Init()
        {

        }

        protected virtual void OnCreate()
        {

        }

        protected virtual void Destroy()
        {

        }

        private void OnDestroy()
        {
            Destroy();
            view.Destory();
            model.Destory();
        }
    }
}
