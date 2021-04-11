using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.UI
{
    public class UIView
    {
        public Transform transform;

        public virtual void OnCreate(Transform transform)
        {
            this.transform = transform;
        }

        public virtual void InitView()
        {

        }

        public virtual void FindObject()
        {

        }

        public virtual void Destory()
        {

        }
    }
}
