using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.UI
{
    /// <summary>
    /// Ê±Ðò  OnCreate  ->   Init    ->   OnUpdate    ->   Destroy
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="M"></typeparam>
    public class UICtrl<V, M> : UIBase where V : UIView, new() where M : UIModel, new()
    {
        public UIModel model;
        public UIView view;

        protected override void OnCreate()
        {
            base.OnCreate();
            model = new V() as UIModel;
            view = new M() as UIView;

            view.OnCreate(transform);
            model.OnCreate();
        }

        // Start is called before the first frame update
        protected override void Init()
        {
            base.Init();
            view.FindObject();
            view.InitView();
            model.Init();
            Init();
        }

        protected override void Destroy()
        {
            base.Destroy();
            view.Destory();
            model.Destory();
        }
    }
}
