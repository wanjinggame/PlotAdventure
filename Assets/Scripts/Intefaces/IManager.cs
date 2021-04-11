using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plot.Core
{
    public interface IManager
    {
        void Init();
        void Destroy();
        void CommonUpdate();
        void CommonLateUpdate();
        void SetInfo(ManagerInfo info);
        void SetFPS(int fps);
    }
}
