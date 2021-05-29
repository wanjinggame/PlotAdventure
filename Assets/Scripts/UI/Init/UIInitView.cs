using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Plot.UI
{
    public class UIInitView : UIView
    {
        Text dialogText;
        Transform diglogUITran;
        public override void OnCreate(Transform transform)
        {
            base.OnCreate(transform);
        }

        public override void FindObject()
        {
            base.FindObject();
            diglogUITran = this.transform.Find("dialogUI");
            Transform textTran = diglogUITran.Find("dialogText");
            dialogText = textTran.GetComponent<Text>();
        }

        public void InitBtn()
        {
            
        }
        public override void InitView()
        {
            base.InitView();
        }


        public override void Destory()
        {
            base.Destory();
        }
    }
}
